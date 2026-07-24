namespace SlojPodataka.BibliotekaKlasa.KlasePodataka.Modeli
{
    public class ProizvodniNalogModel
    {
        public int IdNaloga { get; set; }
        public string? BrojNaloga { get; set; }

        public int IdKupac { get; set; }
        public KupacModel? KupacObjekat { get; set; }

        public int IdKorisnik { get; set; }
        public KorisnikModel? KorisnikObjekat { get; set; }

        public DateTime DatumPrijema { get; set; }
        public DateTime ZeljeniDatumIzrade { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }
        public string Status { get; set; } = "Na cekanju";
        public decimal UkupnaPovrsinaM2 { get; set; }

        public List<StavkaNalogaModel> Stavke { get; set; } = new List<StavkaNalogaModel>();
    }
}
