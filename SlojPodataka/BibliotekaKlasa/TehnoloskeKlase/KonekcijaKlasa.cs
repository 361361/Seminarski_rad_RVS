using Microsoft.Data.SqlClient;

namespace BibliotekaKlasa.TehnoloskeKlase
{
    // CRC: Odgovornost - upravljanje konekcijom ka bazi podataka (SQL Server).
    // Koristi se u DBUtils (TabelaKlasa) i u standardnim SqlClient repozitorijumima.
    public class KonekcijaKlasa
    {
        private SqlConnection? _konekcija;
        private readonly string _stringKonekcije;

        public KonekcijaKlasa(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        public bool OtvoriKonekciju()
        {
            bool uspeh;
            _konekcija = new SqlConnection(_stringKonekcije);
            try
            {
                _konekcija.Open();
                uspeh = true;
            }
            catch
            {
                uspeh = false;
            }
            return uspeh;
        }

        public SqlConnection DajKonekciju()
        {
            return _konekcija!;
        }

        public void ZatvoriKonekciju()
        {
            if (_konekcija != null && _konekcija.State == System.Data.ConnectionState.Open)
            {
                _konekcija.Close();
            }
        }
    }
}
