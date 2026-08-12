using Microsoft.AspNetCore.Mvc;
using BibliotekaKlasa.KlasePodatakaSP.Repozitorijumi;
using BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Controllers
{
    // NAPOMENA O NAZIVU: "Nalog" ovde znači KORISNIČKI NALOG (prijava/login),
    // ne meša se sa ProizvodniNalogController, koji upravlja proizvodnim nalozima
    // za izradu PVC stolarije. Isto imenovanje kao u originalnom šablonu predmeta.
    public class NalogController : Controller
    {
        private readonly KorisnikSPRepo _korisnikRepo;

        public NalogController(KorisnikSPRepo korisnikRepoObjekat)
        {
            _korisnikRepo = korisnikRepoObjekat;
        }

        [HttpGet]
        public IActionResult Prijava()
        {
            if (HttpContext.Session.GetInt32("KorisnikId") != null)
            {
                return RedirectToAction("Index", "ProizvodniNalog");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Prijava(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var korisnik = _korisnikRepo.DajPoKorisnickomImenu(model.KorisnickoIme);

            bool ispravnaLozinka = korisnik != null
                && !string.IsNullOrEmpty(korisnik.LozinkaSalt)
                && !string.IsNullOrEmpty(korisnik.LozinkaHash)
                && FunkcijeLozinke.ProveriLozinku(model.Lozinka, korisnik.LozinkaSalt, korisnik.LozinkaHash);

            if (!ispravnaLozinka)
            {
                ModelState.AddModelError(string.Empty, "Pogrešno korisničko ime ili lozinka.");
                return View(model);
            }

            HttpContext.Session.SetInt32("KorisnikId", korisnik!.Id);
            HttpContext.Session.SetString("KorisnickoIme", korisnik.KorisnickoIme ?? string.Empty);
            HttpContext.Session.SetString("ImePrezime", $"{korisnik.Ime} {korisnik.Prezime}");
            HttpContext.Session.SetString("Uloga", korisnik.Uloga ?? "Referent");

            return RedirectToAction("Index", "ProizvodniNalog");
        }

        public IActionResult Odjava()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Prijava));
        }

        public IActionResult Greska() => View();
    }
}
