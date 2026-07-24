namespace SlojPodataka.BibliotekaKlasa.KlasePodataka.Modeli
{
    public class StavkaNalogaModel
    {
        public int IdStavke { get; set; }
        public int IdNaloga { get; set; }
        public string? TipElementa { get; set; }   
        public int SirinaMM { get; set; }
        public int VisinaMM { get; set; }
        public int Kolicina { get; set; }
        public string? BojaProfila { get; set; }
        public string? TipStakla { get; set; }
        public string? TipOkova { get; set; }

               public decimal PovrsinaM2 =>
            Math.Round((SirinaMM / 1000m) * (VisinaMM / 1000m) * Kolicina, 2);
    }
}
