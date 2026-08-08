using Microsoft.AspNetCore.Mvc;
using RVS_Aplikacija.ViewModels;
using RVS_Aplikacija.Servisi;
using RVS_Aplikacija.Filteri;

namespace RVS_Aplikacija.Controllers
{
    // CRUD nad šifarnikom kupaca - poziva REST API (KupacApiController), koji
    // koristi KupacRepo (Način 2 - DBUtils/TabelaKlasa nasleđivanje) iz Sloja 1.
    [ZahtevaPrijavu]
    public class KupacController : Controller
    {
        private readonly KupacApiServis _kupacServis;

        public KupacController(KupacApiServis kupacServisObjekat)
        {
            _kupacServis = kupacServisObjekat;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            var lista = await _kupacServis.DajSveAsync(filter);
            ViewBag.Filter = filter;
            return View(lista);
        }

        [HttpGet]
        public IActionResult Dodaj() => View(new KupacViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(KupacViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool uspeh = await _kupacServis.DodajAsync(model);
            if (!uspeh)
            {
                ModelState.AddModelError(string.Empty, "Greška prilikom čuvanja kupca.");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Izmeni(int id)
        {
            var kupac = await _kupacServis.DajPoIdAsync(id);
            if (kupac == null) return NotFound();
            return View(kupac);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Izmeni(KupacViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            bool uspeh = await _kupacServis.IzmeniAsync(model);
            if (!uspeh)
            {
                ModelState.AddModelError(string.Empty, "Greška prilikom izmene kupca.");
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Obrisi(int id)
        {
            await _kupacServis.ObrisiAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
