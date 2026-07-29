using BibliotekaKlasa.KlasePodataka.Modeli;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BibliotekaKlasa.KlasePodatakaSP.Repozitorijumi
{
    // NAČIN 3 realizacije sloja za rad sa podacima:
    // standardne SqlClient klase u kombinaciji sa uskladištenim procedurama (stored procedures).
    public class KorisnikSPRepo
    {
        private readonly string _stringKonekcije;

        public KorisnikSPRepo(string noviStringKonekcije)
        {
            _stringKonekcije = noviStringKonekcije;
        }

        public int DodajKorisnika(KorisnikModel korisnikObjekat)
        {
            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spDodajKorisnika", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@KorisnickoIme", SqlDbType.NVarChar).Value = korisnikObjekat.KorisnickoIme;
            komanda.Parameters.Add("@Ime", SqlDbType.NVarChar).Value = korisnikObjekat.Ime;
            komanda.Parameters.Add("@Prezime", SqlDbType.NVarChar).Value = korisnikObjekat.Prezime;
            komanda.Parameters.Add("@Email", SqlDbType.NVarChar).Value = (object?)korisnikObjekat.Email ?? DBNull.Value;
            komanda.Parameters.Add("@LozinkaHash", SqlDbType.NVarChar).Value = korisnikObjekat.LozinkaHash;
            komanda.Parameters.Add("@LozinkaSalt", SqlDbType.NVarChar).Value = korisnikObjekat.LozinkaSalt;
            komanda.Parameters.Add("@Uloga", SqlDbType.NVarChar).Value = korisnikObjekat.Uloga;

            var noviId = komanda.ExecuteScalar();
            return Convert.ToInt32(noviId);
        }

        public KorisnikModel? DajPoKorisnickomImenu(string korisnickoIme)
        {
            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spDajKorisnikPoKorisnickomImenu", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@KorisnickoIme", SqlDbType.NVarChar).Value = korisnickoIme;

            using var citac = komanda.ExecuteReader();
            if (citac.Read())
            {
                return MapirajCitac(citac);
            }
            return null;
        }

        public KorisnikModel? DajPoId(int id)
        {
            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spDajKorisnikaPoId", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var citac = komanda.ExecuteReader();
            if (citac.Read())
            {
                return MapirajCitac(citac);
            }
            return null;
        }

        public List<KorisnikModel> DajSve()
        {
            var lista = new List<KorisnikModel>();

            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spDajSveKorisnike", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;

            using var citac = komanda.ExecuteReader();
            while (citac.Read())
            {
                lista.Add(MapirajCitac(citac));
            }
            return lista;
        }

        public bool IzmeniKorisnika(KorisnikModel korisnikObjekat)
        {
            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spIzmeniKorisnika", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = korisnikObjekat.Id;
            komanda.Parameters.Add("@Ime", SqlDbType.NVarChar).Value = korisnikObjekat.Ime;
            komanda.Parameters.Add("@Prezime", SqlDbType.NVarChar).Value = korisnikObjekat.Prezime;
            komanda.Parameters.Add("@Email", SqlDbType.NVarChar).Value = (object?)korisnikObjekat.Email ?? DBNull.Value;
            komanda.Parameters.Add("@Uloga", SqlDbType.NVarChar).Value = korisnikObjekat.Uloga;

            return komanda.ExecuteNonQuery() > 0;
        }

        public bool ObrisiKorisnika(int id)
        {
            using var konekcija = new SqlConnection(_stringKonekcije);
            konekcija.Open();

            using var komanda = new SqlCommand("spObrisiKorisnika", konekcija);
            komanda.CommandType = CommandType.StoredProcedure;
            komanda.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            return komanda.ExecuteNonQuery() > 0;
        }

        private static KorisnikModel MapirajCitac(SqlDataReader citac) => new KorisnikModel
        {
            Id = citac.GetInt32(0),
            KorisnickoIme = citac.GetString(1),
            Ime = citac.GetString(2),
            Prezime = citac.GetString(3),
            Email = citac.IsDBNull(4) ? null : citac.GetString(4),
            LozinkaHash = citac.GetString(5),
            LozinkaSalt = citac.GetString(6),
            Uloga = citac.GetString(7)
        };
    }
}
