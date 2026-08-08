using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using RVS_Aplikacija.Servisi;
using RVS_Aplikacija.Filteri;

namespace RVS_Aplikacija.Controllers
{
    // Glavni kontroler prezentacionog sloja - upravlja proizvodnim nalozima
    // (glavna tabela dokumenta) i njihovim stavkama (master-detail).
    // Sve operacije nad podacima idu preko REST API-ja (NaloziApiServis),
    // koji dalje poziva sloj poslovne logike i sloj za rad sa podacima.
    [ZahtevaPrijavu]
    public class ProizvodniNalogController : Controller
    {
        private readonly NaloziApiServis _naloziServis;
        private readonly KupacApiServis _kupacServis;

        public ProizvodniNalogController(NaloziApiServis naloziServisObjekat, KupacApiServis kupacServisObjekat)
        {
            _naloziServis = naloziServisObjekat;
            _kupacServis = kupacServisObjekat;
        }

        // GET /ProizvodniNalog?filter=...   - tabelarni prikaz sa filterom
        public async Task<IActionResult> Index(string? filter)
        {
            var lista = await _naloziServis.DajSveAsync(filter);
            ViewBag.Filter = filter;
            return View(lista);
        }

        // GET /ProizvodniNalog/Detalji/5   - prikaz pojedinačnog zapisa sa svim stavkama
        public async Task<IActionResult> Detalji(int id)
        {
            var nalog = await _naloziServis.DajPoIdAsync(id);
            if (nalog == null) return NotFound();
            return View(nalog);
        }

        // GET /ProizvodniNalog/Kreiraj   - forma za unos (zaglavlje + stavke na jednom ekranu)
        public async Task<IActionResult> Kreiraj()
        {
            var model = new NoviNalogViewModel
            {
                IdKorisnik = HttpContext.Session.GetInt32("KorisnikId") ?? 0,
                DostupniKupci = await _kupacServis.DajSveAsync()
            };
            return View(model);
        }

        // POST /ProizvodniNalog/Kreiraj
        // Unos cele celine (zaglavlje+stavke) na jednoj formi. Provera poslovnog pravila
        // i sama transakcija upisa dešavaju se u REST API / poslovnoj logici (Deo 2 i 3);
        // ovde se samo prosleđuju podaci i prikazuje rezultat korisniku.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kreiraj(NoviNalogViewModel model)
        {
            model.IdKorisnik = HttpContext.Session.GetInt32("KorisnikId") ?? 0;

            if (!ModelState.IsValid)
            {
                model.DostupniKupci = await _kupacServis.DajSveAsync();
                return View(model);
            }

            var rezultat = await _naloziServis.KreirajAsync(model);
            if (rezultat == null)
            {
                ModelState.AddModelError(string.Empty, "Greška prilikom kreiranja naloga. Proveri unete podatke i pokušaj ponovo.");
                model.DostupniKupci = await _kupacServis.DajSveAsync();
                return View(model);
            }

            // Poruka o rezultatu primene poslovnog pravila (da li je datum pomeren)
            TempData["Napomena"] = rezultat.DatumJePomeren
                ? $"Kapacitet za {rezultat.ZeljeniDatumIzrade:dd.MM.yyyy.} je popunjen. " +
                  $"Predloženi novi termin izrade: {rezultat.PredlozeniDatumIzrade:dd.MM.yyyy.}"
                : $"Nalog {rezultat.BrojNaloga} je uspešno kreiran za {rezultat.PredlozeniDatumIzrade:dd.MM.yyyy.}";

            return RedirectToAction(nameof(Detalji), new { id = rezultat.IdNaloga });
        }

        // POST /ProizvodniNalog/IzmeniStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IzmeniStatus(int id, string noviStatus)
        {
            await _naloziServis.IzmeniStatusAsync(id, noviStatus);
            return RedirectToAction(nameof(Detalji), new { id });
        }

        // POST /ProizvodniNalog/Obrisi/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id)
        {
            await _naloziServis.ObrisiAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET /ProizvodniNalog/Stampa?filter=...   - štampa spiska svih (ili filtriranih) naloga
        public async Task<IActionResult> Stampa(string? filter)
        {
            var lista = await _naloziServis.DajSveAsync(filter);
            ViewBag.Filter = filter;
            return View(lista);
        }

        // GET /ProizvodniNalog/StampaPojedinacna/5   - parametarska štampa jednog dokumenta
        // (izgled prati "Proizvodni nalog" dokument definisan u seminarskom radu)
        public async Task<IActionResult> StampaPojedinacna(int id)
        {
            var nalog = await _naloziServis.DajPoIdAsync(id);
            if (nalog == null) return NotFound();
            return View(nalog);
        }
    }
}
