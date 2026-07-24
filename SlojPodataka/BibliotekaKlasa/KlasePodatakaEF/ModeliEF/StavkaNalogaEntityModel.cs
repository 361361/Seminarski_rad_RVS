using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaKlasa.KlasePodatakaEF.ModeliEF
{
    [Table("StavkaNaloga")]
    public class StavkaNalogaEntityModel
    {
        [Key]
        public int IdStavke { get; set; }

        public int IdNaloga { get; set; }
        [ForeignKey(nameof(IdNaloga))]
        public ProizvodniNalogEntityModel? Nalog { get; set; }

        [Required, MaxLength(30)]
        public string TipElementa { get; set; } = string.Empty;

        public int SirinaMM { get; set; }
        public int VisinaMM { get; set; }
        public int Kolicina { get; set; }

        [MaxLength(50)]
        public string? BojaProfila { get; set; }
        [MaxLength(50)]
        public string? TipStakla { get; set; }
        [MaxLength(50)]
        public string? TipOkova { get; set; }

        // Computed kolona u bazi (PERSISTED) - EF je čita, ne upisuje
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(6,2)")]
        public decimal PovrsinaM2 { get; set; }
    }
}
