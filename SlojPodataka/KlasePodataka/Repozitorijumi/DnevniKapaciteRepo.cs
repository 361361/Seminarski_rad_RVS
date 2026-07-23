using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.TehnoloskeKlase;
using Microsoft.Data.SqlClient;

namespace BibliotekaKlasa.KlasePodataka.Repozitorijumi
{
    // Standardna SqlClient realizacija (kao pomoćni repozitorijum, koristi ga
    // sloj poslovne logike / REST servis za proveru i ažuriranje kapaciteta).
    public class DnevniKapacitetRepo
    {
        private readonly KonekcijaKlasa _konekcijaObjekat;

        public DnevniKapacitetRepo(KonekcijaKlasa konekcijaObjekat)
        {
            _konekcijaObjekat = konekcijaObjekat;
        }

        // Vraća evidenciju za dati datum; ako ne postoji, vraća "prazan" zapis (0 iskorišćeno)
        public DnevniKapacitetModel DajZaDatum(DateTime datum)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = "SELECT Datum, IskoriscenoM2, BrojNaloga FROM DnevniKapacitet WHERE Datum=@Datum";

            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Datum", datum.Date);

            DnevniKapacitetModel rezultat = new DnevniKapacitetModel { Datum = datum.Date, IskoriscenoM2 = 0, BrojNaloga = 0 };

            using (var citac = komanda.ExecuteReader())
            {
                if (citac.Read())
                {
                    rezultat.Datum = citac.GetDateTime(0);
                    rezultat.IskoriscenoM2 = citac.GetDecimal(1);
                    rezultat.BrojNaloga = citac.GetInt32(2);
                }
            }
            _konekcijaObjekat.ZatvoriKonekciju();
            return rezultat;
        }

        // Uvećava iskorišćenu površinu za dati datum (kreira zapis ako ne postoji - "upsert")
        public void AzurirajIskoriscenost(DateTime datum, decimal dodatnaPovrsina)
        {
            _konekcijaObjekat.OtvoriKonekciju();
            string upit = @"
                MERGE DnevniKapacitet AS cilj
                USING (SELECT @Datum AS Datum) AS izvor
                ON cilj.Datum = izvor.Datum
                WHEN MATCHED THEN
                    UPDATE SET IskoriscenoM2 = cilj.IskoriscenoM2 + @Povrsina,
                               BrojNaloga = cilj.BrojNaloga + 1
                WHEN NOT MATCHED THEN
                    INSERT (Datum, IskoriscenoM2, BrojNaloga)
                    VALUES (@Datum, @Povrsina, 1);";

            using var komanda = new SqlCommand(upit, _konekcijaObjekat.DajKonekciju());
            komanda.Parameters.AddWithValue("@Datum", datum.Date);
            komanda.Parameters.AddWithValue("@Povrsina", dodatnaPovrsina);
            komanda.ExecuteNonQuery();
            _konekcijaObjekat.ZatvoriKonekciju();
        }
    }
}
