using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("ProizvodniNalog")]
    public class ProizvodniNalogEntityModel
    {
        [Key]
        public int IdNaloga { get; set; }

        [Required, MaxLength(20)]
        public string BrojNaloga { get; set; } = string.Empty;

        public int IdKupac { get; set; }
        [ForeignKey(nameof(IdKupac))]
        public KupacEntityModel? Kupac { get; set; }

        public int IdKorisnik { get; set; }

        public DateTime DatumPrijema { get; set; }
        public DateTime ZeljeniDatumIzrade { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }

        [MaxLength(30)]
        public string Status { get; set; } = "Na cekanju";

        [Column(TypeName = "decimal(8,2)")]
        public decimal UkupnaPovrsinaM2 { get; set; }

        // Master-detail: jedan nalog -> više stavki (1:N)
        public List<StavkaNalogaEntityModel> Stavke { get; set; } = new List<StavkaNalogaEntityModel>();
    }
}
