using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BibliotekaKlasa.KlasePodatakaEF.ModeliEF;
using BibliotekaKlasa.KlasePodatakaEF.KontekstEF;

namespace BibliotekaKlasa.KlasePodatakaEF.RepozitorijumiEF
{
    // NAČIN 1 realizacije sloja za rad sa podacima: Entity Framework Core.
    // Ovaj repozitorijum se koristi za glavnu tabelu (ProizvodniNalog) sa
    // master-detail odnosom prema StavkaNaloga - unos u okviru transakcije.
    public class ProizvodniNalogRepo
    {
        private readonly AppDbContext _kontekst;

        public ProizvodniNalogRepo(AppDbContext kontekst)
        {
            _kontekst = kontekst;
        }

        // Unos celine (zaglavlje + stavke) u okviru jedne transakcije
        public int DodajSaStavkama(ProizvodniNalogEntityModel nalogEntityObjekat)
        {
            using IDbContextTransaction transakcija = _kontekst.Database.BeginTransaction();
            try
            {
                _kontekst.ProizvodniNalogEntityModelObjektiDBSet.Add(nalogEntityObjekat);
                _kontekst.SaveChanges();   // upisuje zaglavlje i sve stavke (EF prati navigaciju)

                transakcija.Commit();
                return nalogEntityObjekat.IdNaloga;
            }
            catch
            {
                transakcija.Rollback();
                throw;
            }
        }

        public void Izmeni(ProizvodniNalogEntityModel nalogEntityObjekat)
        {
            _kontekst.ProizvodniNalogEntityModelObjektiDBSet.Update(nalogEntityObjekat);
            _kontekst.SaveChanges();
        }

        public void IzmeniStatus(int idNaloga, string noviStatus)
        {
            var nalog = _kontekst.ProizvodniNalogEntityModelObjektiDBSet.Find(idNaloga);
            if (nalog == null) return;
            nalog.Status = noviStatus;
            _kontekst.SaveChanges();
        }

        public void Obrisi(int idNaloga)
        {
            var nalog = _kontekst.ProizvodniNalogEntityModelObjektiDBSet
                .Include(n => n.Stavke)
                .FirstOrDefault(n => n.IdNaloga == idNaloga);
            if (nalog == null) return;

            _kontekst.ProizvodniNalogEntityModelObjektiDBSet.Remove(nalog); // kaskadno briše i stavke
            _kontekst.SaveChanges();
        }

        public List<ProizvodniNalogEntityModel> DajSve()
        {
            return _kontekst.ProizvodniNalogEntityModelObjektiDBSet
                .Include(n => n.Kupac)
                .Include(n => n.Stavke)
                .OrderByDescending(n => n.DatumPrijema)
                .ToList();
        }

        public List<ProizvodniNalogEntityModel> DajSveSaFilterom(string filter)
        {
            return _kontekst.ProizvodniNalogEntityModelObjektiDBSet
                .Include(n => n.Kupac)
                .Include(n => n.Stavke)
                .Where(n => n.BrojNaloga.Contains(filter)
                         || (n.Kupac != null && n.Kupac.Naziv.Contains(filter))
                         || n.Status.Contains(filter))
                .OrderByDescending(n => n.DatumPrijema)
                .ToList();
        }

        // Pojedinačni zapis sa svim delovima (za prikaz, štampu, parametarsku štampu)
        public ProizvodniNalogEntityModel? DajPoIdSaStavkama(int idNaloga)
        {
            return _kontekst.ProizvodniNalogEntityModelObjektiDBSet
                .Include(n => n.Kupac)
                .Include(n => n.Stavke)
                .FirstOrDefault(n => n.IdNaloga == idNaloga);
        }

        public string SledeciBrojNaloga()
        {
            int godina = DateTime.Now.Year;
            int brojUGodini = _kontekst.ProizvodniNalogEntityModelObjektiDBSet
                .Count(n => n.DatumPrijema.Year == godina) + 1;
            return $"PN-{godina}-{brojUGodini:D6}";
        }
    }
}
