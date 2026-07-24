using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotekaKlasa.TehnoloskeKlase
{
    // CRC: Odgovornost - generička (DBUtils) klasa za rad sa proizvoljnim SQL upitima
    // nad jednom tabelom, koju repozitorijumi nasleđuju (npr. KupacRepo : TabelaKlasa).
    // Ovo je "Način 2" realizacije sloja za rad sa podacima (nasleđivanje DBUtils klase + upiti).
    public class TabelaKlasa
    {
        private readonly string _nazivTabele;
        private readonly KonekcijaKlasa _konekcijaObjekat;
        private SqlDataAdapter? _adapterObjekat;
        private DataSet? _dataSetObjekat;

        public TabelaKlasa(KonekcijaKlasa novaKonekcija, string noviNazivTabele)
        {
            _konekcijaObjekat = novaKonekcija;
            _nazivTabele = noviNazivTabele;
        }

        private void KreirajAdapter(string selectUpit)
        {
            var selectKomanda = new SqlCommand(selectUpit, _konekcijaObjekat.DajKonekciju());
            _adapterObjekat = new SqlDataAdapter { SelectCommand = selectKomanda };
        }

        private void KreirajDataset()
        {
            _dataSetObjekat = new DataSet();
            _adapterObjekat!.Fill(_dataSetObjekat, _nazivTabele);
        }

        // Vraća podatke za dati SELECT upit
        public DataSet DajPodatke(string selectUpit)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            KreirajAdapter(selectUpit);
            KreirajDataset();
            _konekcijaObjekat.ZatvoriKonekciju();
            return _dataSetObjekat!;
        }

        // Izvršava INSERT/UPDATE/DELETE upit unutar transakcije
        public bool IzvrsiAzuriranje(string upit)
        {
            bool uspeh;
            _konekcijaObjekat.OtvoriKonekciju();
            SqlConnection konekcija = _konekcijaObjekat.DajKonekciju();
            SqlTransaction? transakcija = null;
            try
            {
                var komanda = konekcija.CreateCommand();
                transakcija = konekcija.BeginTransaction();
                komanda.Transaction = transakcija;
                komanda.CommandText = upit;
                komanda.ExecuteNonQuery();
                transakcija.Commit();
                uspeh = true;
            }
            catch
            {
                transakcija?.Rollback();
                uspeh = false;
            }
            finally
            {
                _konekcijaObjekat.ZatvoriKonekciju();
            }
            return uspeh;
        }

        // Preklopljena (overload) varijanta - izvršava više upita u jednoj transakciji
        public bool IzvrsiAzuriranje(List<string> listaUpita)
        {
            bool uspeh;
            _konekcijaObjekat.OtvoriKonekciju();
            SqlConnection konekcija = _konekcijaObjekat.DajKonekciju();
            SqlTransaction? transakcija = null;
            try
            {
                var komanda = konekcija.CreateCommand();
                transakcija = konekcija.BeginTransaction();
                komanda.Transaction = transakcija;
                foreach (var upit in listaUpita)
                {
                    komanda.CommandText = upit;
                    komanda.ExecuteNonQuery();
                }
                transakcija.Commit();
                uspeh = true;
            }
            catch
            {
                transakcija?.Rollback();
                uspeh = false;
            }
            finally
            {
                _konekcijaObjekat.ZatvoriKonekciju();
            }
            return uspeh;
        }
    }
}
