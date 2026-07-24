namespace SlojPodataka.BibliotekaKlasa.KlasePodataka.Modeli
{
    public class KorisnikModel
    {
        public int Id { get; set; }
        public string? KorisnickoIme { get; set; }
        public string? Ime { get; set; }
        public string? Prezime { get; set; }
        public string? Email { get; set; }
        public string? LozinkaHash { get; set; }
        public string? LozinkaSalt { get; set; }
        public string? Uloga { get; set; }   
    }
}
