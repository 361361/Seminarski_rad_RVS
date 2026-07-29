using Microsoft.AspNetCore.Mvc;

using BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using PoslovnaLogika.Klase;
using RVS_REST_API.DTO;

namespace RVS_REST_API.Controllers
{
    // Sloj servisa - međusloj koji prezentacionom sloju obezbeđuje CRUD operacije
    // nad glavnom tabelom (ProizvodniNalog), i istovremeno je mesto gde se poziva
    // sloj poslovne logike da primeni poslovno pravilo pre upisa u bazu.
    [ApiController]
    [Route("api/[controller]")]
    public class NaloziApiController : ControllerBase
    {
        private readonly ProizvodniNalogRepo _nalogRepo;
        private readonly ProizvodniNalogLogika _nalogLogika;

        public NaloziApiController(ProizvodniNalogRepo nalogRepoObjekat, ProizvodniNalogLogika nalogLogikaObjekat)
        {
            _nalogRepo = nalogRepoObjekat;
            _nalogLogika = nalogLogikaObjekat;
        }

        // GET api/NaloziApi?filter=Petrovic
        [HttpGet]
        public ActionResult<List<ProizvodniNalogEntityModel>> DajSve([FromQuery] string? filter)
        {
            var lista = string.IsNullOrWhiteSpace(filter)
                ? _nalogRepo.DajSve()
                : _nalogRepo.DajSveSaFilterom(filter);

            return Ok(lista);
        }

        // GET api/NaloziApi/5
        [HttpGet("{id}")]
        public ActionResult<ProizvodniNalogEntityModel> DajPoId(int id)
        {
            var nalog = _nalogRepo.DajPoIdSaStavkama(id);
            if (nalog == null) return NotFound();
            return Ok(nalog);
        }

        // POST api/NaloziApi
        // Kreira novi nalog: primenjuje poslovno pravilo (predlaže datum ako je potrebno),
        // upisuje zaglavlje+stavke u transakciji, ažurira evidenciju kapaciteta.
        [HttpPost]
        public ActionResult<NalogKreiranOdgovorDto> Kreiraj([FromBody] NoviNalogDto dto)
        {
            if (dto.Stavke == null || dto.Stavke.Count == 0)
            {
                return BadRequest("Nalog mora sadržati bar jednu stavku.");
            }

            // 1) Primena poslovnog pravila (Sloj poslovne logike)
            var stavkeZaProveru = dto.Stavke.Select(s => (s.SirinaMM, s.VisinaMM, s.Kolicina));
            RezultatPlaniranjaNaloga rezultat;
            try
            {
                rezultat = _nalogLogika.PripremiPlanNaloga(dto.ZeljeniDatumIzrade, stavkeZaProveru);
            }
            catch (ArgumentException greska)
            {
                return BadRequest(greska.Message);
            }

            // 2) Mapiranje DTO -> EF entitet i upis u bazu (Sloj za rad sa podacima)
            var nalogEntity = new ProizvodniNalogEntityModel
            {
                BrojNaloga = _nalogRepo.SledeciBrojNaloga(),
                IdKupac = dto.IdKupac,
                IdKorisnik = dto.IdKorisnik,
                DatumPrijema = DateTime.Today,
                ZeljeniDatumIzrade = dto.ZeljeniDatumIzrade,
                PredlozeniDatumIzrade = rezultat.PredlozeniDatumIzrade,
                Status = "Na cekanju",
                UkupnaPovrsinaM2 = rezultat.UkupnaPovrsinaM2,
                Stavke = dto.Stavke.Select(s => new StavkaNalogaEntityModel
                {
                    TipElementa = s.TipElementa,
                    SirinaMM = s.SirinaMM,
                    VisinaMM = s.VisinaMM,
                    Kolicina = s.Kolicina,
                    BojaProfila = s.BojaProfila,
                    TipStakla = s.TipStakla,
                    TipOkova = s.TipOkova
                }).ToList()
            };

            int noviId = _nalogRepo.DodajSaStavkama(nalogEntity);

            // 3) Ažuriranje evidencije kapaciteta (opet poslovna logika)
            _nalogLogika.PotvrdiRezervacijuKapaciteta(rezultat.PredlozeniDatumIzrade, rezultat.UkupnaPovrsinaM2);

            var odgovor = new NalogKreiranOdgovorDto
            {
                IdNaloga = noviId,
                BrojNaloga = nalogEntity.BrojNaloga,
                ZeljeniDatumIzrade = dto.ZeljeniDatumIzrade,
                PredlozeniDatumIzrade = rezultat.PredlozeniDatumIzrade,
                DatumJePomeren = rezultat.DatumJePomeren,
                UkupnaPovrsinaM2 = rezultat.UkupnaPovrsinaM2
            };

            return CreatedAtAction(nameof(DajPoId), new { id = noviId }, odgovor);
        }

        // PUT api/NaloziApi/5/status
        [HttpPut("{id}/status")]
        public IActionResult IzmeniStatus(int id, [FromBody] string noviStatus)
        {
            var nalog = _nalogRepo.DajPoIdSaStavkama(id);
            if (nalog == null) return NotFound();

            _nalogRepo.IzmeniStatus(id, noviStatus);
            return NoContent();
        }

        // DELETE api/NaloziApi/5
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            var nalog = _nalogRepo.DajPoIdSaStavkama(id);
            if (nalog == null) return NotFound();

            _nalogRepo.Obrisi(id);
            return NoContent();
        }
    }
}
