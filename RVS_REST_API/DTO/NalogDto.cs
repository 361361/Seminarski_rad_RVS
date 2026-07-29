namespace RVS_REST_API.DTO
{
    // Podaci koje prezentacioni sloj šalje kad kreira novi proizvodni nalog
    public class NoviNalogDto
    {
        public int IdKupac { get; set; }
        public int IdKorisnik { get; set; }
        public DateTime ZeljeniDatumIzrade { get; set; }
        public List<StavkaDto> Stavke { get; set; } = new List<StavkaDto>();
    }

    public class StavkaDto
    {
        public string TipElementa { get; set; } = string.Empty;
        public int SirinaMM { get; set; }
        public int VisinaMM { get; set; }
        public int Kolicina { get; set; }
        public string? BojaProfila { get; set; }
        public string? TipStakla { get; set; }
        public string? TipOkova { get; set; }
    }

    // Odgovor posle kreiranja naloga - uključuje napomenu o eventualnoj promeni datuma
    public class NalogKreiranOdgovorDto
    {
        public int IdNaloga { get; set; }
        public string BrojNaloga { get; set; } = string.Empty;
        public DateTime ZeljeniDatumIzrade { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }
        public bool DatumJePomeren { get; set; }
        public decimal UkupnaPovrsinaM2 { get; set; }
    }

    // Odgovor za samostalnu proveru kapaciteta (bez kreiranja naloga)
    public class ProveraKapacitetaOdgovorDto
    {
        public DateTime TrazeniDatum { get; set; }
        public DateTime PredlozeniDatum { get; set; }
        public bool DatumJePomeren { get; set; }
    }

    public class KupacDto
    {
        public int IdKupac { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Adresa { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }
    }
}
