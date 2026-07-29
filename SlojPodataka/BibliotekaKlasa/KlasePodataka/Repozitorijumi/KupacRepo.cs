using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.TehnoloskeKlase;
using System.Data;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    // NAČIN 2 realizacije sloja za rad sa podacima:
    // nasleđivanje DBUtils (TabelaKlasa) klase i primena SQL upita nad njom.
    public class KupacRepo : TabelaKlasa
    {
        public KupacRepo(KonekcijaKlasa konekcijaObjekat) : base(konekcijaObjekat, "Kupac")
        {
        }

        public void Dodaj(KupacModel kupacObjekat)
        {
            string upit =
                "INSERT INTO Kupac (Naziv, Adresa, Telefon, Email) VALUES (" +
                "'" + kupacObjekat.Naziv + "', " +
                "'" + kupacObjekat.Adresa + "', " +
                "'" + kupacObjekat.Telefon + "', " +
                "'" + kupacObjekat.Email + "')";

            this.IzvrsiAzuriranje(upit);
        }

        public void Izmeni(KupacModel kupacObjekat)
        {
            string upit =
                "UPDATE Kupac SET " +
                "Naziv='" + kupacObjekat.Naziv + "', " +
                "Adresa='" + kupacObjekat.Adresa + "', " +
                "Telefon='" + kupacObjekat.Telefon + "', " +
                "Email='" + kupacObjekat.Email + "' " +
                "WHERE IdKupac=" + kupacObjekat.IdKupac;

            this.IzvrsiAzuriranje(upit);
        }

        public void Obrisi(int idKupac)
        {
            string upit = "DELETE FROM Kupac WHERE IdKupac=" + idKupac;
            this.IzvrsiAzuriranje(upit);
        }

        public List<KupacModel> DajSve()
        {
            var lista = new List<KupacModel>();
            string upit = "SELECT IdKupac, Naziv, Adresa, Telefon, Email FROM Kupac ORDER BY Naziv";

            DataSet kolekcija = this.DajPodatke(upit);
            foreach (DataRow red in kolekcija.Tables[0].Rows)
            {
                lista.Add(MapirajRed(red));
            }
            return lista;
        }

        public List<KupacModel> DajSveSaFilterom(string filter)
        {
            var lista = new List<KupacModel>();
            string upit =
                "SELECT IdKupac, Naziv, Adresa, Telefon, Email FROM Kupac " +
                "WHERE Naziv LIKE '%" + filter + "%' OR Email LIKE '%" + filter + "%' " +
                "ORDER BY Naziv";

            DataSet kolekcija = this.DajPodatke(upit);
            foreach (DataRow red in kolekcija.Tables[0].Rows)
            {
                lista.Add(MapirajRed(red));
            }
            return lista;
        }

        public KupacModel? DajPoId(int idKupac)
        {
            string upit = "SELECT IdKupac, Naziv, Adresa, Telefon, Email FROM Kupac WHERE IdKupac=" + idKupac;
            DataSet kolekcija = this.DajPodatke(upit);

            if (kolekcija.Tables[0].Rows.Count == 0) return null;
            return MapirajRed(kolekcija.Tables[0].Rows[0]);
        }

        private static KupacModel MapirajRed(DataRow red) => new KupacModel
        {
            IdKupac = Convert.ToInt32(red[0]),
            Naziv = red[1].ToString(),
            Adresa = red[2].ToString(),
            Telefon = red[3].ToString(),
            Email = red[4].ToString()
        };
    }
}
