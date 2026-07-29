using Microsoft.AspNetCore.Mvc;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;
using BibliotekaKlasa.KlasePodataka.Modeli;

namespace RVS_REST_API.Controllers
{
    // CRUD nad šifarnikom Kupac (koristi "Način 2" repozitorijum - KupacRepo : TabelaKlasa)
    [ApiController]
    [Route("api/[controller]")]
    public class KupacApiController : ControllerBase
    {
        private readonly KupacRepo _kupacRepo;

        public KupacApiController(KupacRepo kupacRepoObjekat)
        {
            _kupacRepo = kupacRepoObjekat;
        }

        [HttpGet]
        public ActionResult<List<KupacModel>> DajSve([FromQuery] string? filter)
        {
            var lista = string.IsNullOrWhiteSpace(filter)
                ? _kupacRepo.DajSve()
                : _kupacRepo.DajSveSaFilterom(filter);
            return Ok(lista);
        }

        [HttpGet("{id}")]
        public ActionResult<KupacModel> DajPoId(int id)
        {
            var kupac = _kupacRepo.DajPoId(id);
            if (kupac == null) return NotFound();
            return Ok(kupac);
        }

        [HttpPost]
        public IActionResult Dodaj([FromBody] KupacModel kupacObjekat)
        {
            _kupacRepo.Dodaj(kupacObjekat);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Izmeni(int id, [FromBody] KupacModel kupacObjekat)
        {
            kupacObjekat.IdKupac = id;
            _kupacRepo.Izmeni(kupacObjekat);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            _kupacRepo.Obrisi(id);
            return NoContent();
        }
    }
}
