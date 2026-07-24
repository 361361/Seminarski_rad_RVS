using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BibliotekaKlasa.KlasePodatakaEF.KontekstEF
{
    public class AppDbContext : DbContext
    {
        public DbSet<KupacEntityModel> KupacEntityModelObjektiDBSet { get; set; } = null!;
        public DbSet<ProizvodniNalogEntityModel> ProizvodniNalogEntityModelObjektiDBSet { get; set; } = null!;
        public DbSet<StavkaNalogaEntityModel> StavkaNalogaEntityModelObjektiDBSet { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> opcije) : base(opcije)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Master-detail relacija 1:N sa kaskadnim brisanjem stavki kada se obriše nalog
            modelBuilder.Entity<ProizvodniNalogEntityModel>()
                .HasMany(n => n.Stavke)
                .WithOne(s => s.Nalog)
                .HasForeignKey(s => s.IdNaloga)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProizvodniNalogEntityModel>()
                .HasIndex(n => n.BrojNaloga)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
