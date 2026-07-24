using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("Kupac")]
    public class KupacEntityModel
    {
        [Key]
        public int IdKupac { get; set; }
        public string Naziv { get; set; } = string.Empty;
        public string? Adresa { get; set; }
        public string? Telefon { get; set; }
        public string? Email { get; set; }

        public List<ProizvodniNalogEntityModel>? Nalozi { get; set; }
    }
}
