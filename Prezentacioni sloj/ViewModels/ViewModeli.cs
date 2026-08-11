using System.ComponentModel.DataAnnotations;

namespace RVS_Aplikacija.ViewModels
{
    // LOGIN 
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [Display(Name = "Korisničko ime")]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Lozinka { get; set; } = string.Empty;
    }

    //  KUPAC (šifarnik) 
    public class KupacViewModel
    {
        public int IdKupac { get; set; }

        [Required(ErrorMessage = "Naziv kupca je obavezan.")]
        [StringLength(150, ErrorMessage = "Naziv može imati najviše 150 karaktera.")]
        [Display(Name = "Naziv / Ime i prezime")]
        public string Naziv { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Adresa")]
        public string? Adresa { get; set; }

        [RegularExpression(@"^[0-9\/\-\s\+]{6,20}$", ErrorMessage = "Format telefona nije ispravan (dozvoljene su cifre, razmak, /, -, +).")]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }
        
        [EmailAddress(ErrorMessage = "Email adresa nije ispravnog formata.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }

    // STAVKA NALOGA (detalj - unos) 
    public class StavkaViewModel
    {
        [Required(ErrorMessage = "Tip elementa je obavezan.")]
        [Display(Name = "Tip elementa")]
        public string TipElementa { get; set; } = "Prozor";

        [Range(200, 3000, ErrorMessage = "Širina mora biti između 200 i 3000 mm.")]
        [Display(Name = "Širina (mm)")]
        public int SirinaMM { get; set; }

        [Range(200, 3000, ErrorMessage = "Visina mora biti između 200 i 3000 mm.")]
        [Display(Name = "Visina (mm)")]
        public int VisinaMM { get; set; }

        [Range(1, 100, ErrorMessage = "Količina mora biti između 1 i 100.")]
        [Display(Name = "Količina")]
        public int Kolicina { get; set; } = 1;

        [StringLength(50)]
        [Display(Name = "Boja profila")]
        public string? BojaProfila { get; set; }

        [StringLength(50)]
        [Display(Name = "Tip stakla")]
        public string? TipStakla { get; set; }

        [StringLength(50)]
        [Display(Name = "Tip okova")]
        public string? TipOkova { get; set; }
    }

    // STAVKA NALOGA (prikaz - iz REST API-ja, sa izračunatom površinom) 
    public class StavkaPrikazViewModel
    {
        public int IdStavke { get; set; }
        public string TipElementa { get; set; } = string.Empty;
        public int SirinaMM { get; set; }
        public int VisinaMM { get; set; }
        public int Kolicina { get; set; }
        public string? BojaProfila { get; set; }
        public string? TipStakla { get; set; }
        public string? TipOkova { get; set; }
        public decimal PovrsinaM2 { get; set; }
    }

    // NOVI PROIZVODNI NALOG (unos - master-detail) 
    public class NoviNalogViewModel
    {
        [Required(ErrorMessage = "Morate izabrati kupca.")]
        [Range(1, int.MaxValue, ErrorMessage = "Morate izabrati kupca.")]
        [Display(Name = "Kupac")]
        public int IdKupac { get; set; }

        public int IdKorisnik { get; set; }

        [Required(ErrorMessage = "Željeni datum izrade je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Željeni datum izrade")]
        public DateTime ZeljeniDatumIzrade { get; set; } = DateTime.Today.AddDays(1);

        [MinLength(1, ErrorMessage = "Nalog mora sadržati bar jednu stavku.")]
        public List<StavkaViewModel> Stavke { get; set; } = new List<StavkaViewModel> { new StavkaViewModel() };

        // Popunjava kontroler pre prikaza forme - za padajuću listu kupaca
        public List<KupacViewModel>? DostupniKupci { get; set; }
    }

    // PROIZVODNI NALOG (prikaz - lista, detalji, štampa) 
    public class NalogPrikazViewModel
    {
        public int IdNaloga { get; set; }
        public string BrojNaloga { get; set; } = string.Empty;
        public int IdKupac { get; set; }
        public KupacViewModel? Kupac { get; set; }
        public DateTime DatumPrijema { get; set; }
        public DateTime ZeljeniDatumIzrade { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal UkupnaPovrsinaM2 { get; set; }
        public List<StavkaPrikazViewModel> Stavke { get; set; } = new List<StavkaPrikazViewModel>();
    }

    // ODGOVOR REST API-JA POSLE KREIRANJA NALOGA 
    public class NalogKreiranOdgovor
    {
        public int IdNaloga { get; set; }
        public string BrojNaloga { get; set; } = string.Empty;
        public DateTime ZeljeniDatumIzrade { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }
        public bool DatumJePomeren { get; set; }
        public decimal UkupnaPovrsinaM2 { get; set; }
    }
}
