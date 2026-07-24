using BibliotekaKlasa.KlasePodataka.Modeli;
using BibliotekaKlasa.KlasePodataka.Repozitorijumi;

namespace PoslovnaLogika.Klase
{
    // Klasa predstavlja poslovni proces: "Kreiranje i planiranje proizvodnog naloga
    // za izradu PVC stolarije po porudžbini kupca".
    //
    // Poslovno pravilo (parametrizovano):
    // AKO je ukupna površina (m²) porudžbine, sabrana sa već rezervisanom površinom
    // za traženi datum, veća od maksimalnog dnevnog kapaciteta (K, iz JSON-a),
    // ONDA sistem automatski pronalazi i predlaže naredni slobodan termin izrade.
    public class ProizvodniNalogLogika
    {
        private readonly DnevniKapacitetRepo _kapacitetRepo;
        private readonly OgranicenjeKapaciteta _ogranicenjeObjekat;

        public ProizvodniNalogLogika(DnevniKapacitetRepo kapacitetRepoObjekat, OgranicenjeKapaciteta ogranicenjeObjekat)
        {
            _kapacitetRepo = kapacitetRepoObjekat;
            _ogranicenjeObjekat = ogranicenjeObjekat;
        }

        // Ključna metoda poslovnog pravila:
        //  - poziva "servis" (OgranicenjeKapaciteta) koji obezbeđuje parametar K iz JSON-a
        //  - poziva metodu repozitorijuma (DnevniKapacitetRepo) koja obezbeđuje podatke iz baze
        public DateTime OdrediDatumIzrade(DateTime trazeniDatum, decimal povrsinaNaloga)
        {
            decimal maksimalniKapacitet = _ogranicenjeObjekat.UzmiMaksimalniKapacitetIzJSON();

            DateTime datum = trazeniDatum.Date;
            const int MAX_DANA_PRETRAGE = 60; // zaštita od beskonačne petlje

            for (int i = 0; i < MAX_DANA_PRETRAGE; i++)
            {
                DnevniKapacitetModel kapacitetZaDan = _kapacitetRepo.DajZaDatum(datum);

                bool imaMesta = (kapacitetZaDan.IskoriscenoM2 + povrsinaNaloga) <= maksimalniKapacitet;
                if (imaMesta)
                {
                    return datum;
                }

                datum = datum.AddDays(1);
            }

            throw new InvalidOperationException(
                $"Nije pronađen slobodan proizvodni termin u narednih {MAX_DANA_PRETRAGE} dana.");
        }

        // Izračunava ukupnu površinu porudžbine na osnovu unetih stavki (dimenzije u mm)
        public decimal IzracunajUkupnuPovrsinu(IEnumerable<(int sirinaMM, int visinaMM, int kolicina)> stavke)
        {
            decimal ukupno = 0;
            foreach (var stavka in stavke)
            {
                ukupno += (stavka.sirinaMM / 1000m) * (stavka.visinaMM / 1000m) * stavka.kolicina;
            }
            return Math.Round(ukupno, 2);
        }

        // Priprema podataka za kreiranje naloga: izračunava površinu i primenjuje poslovno pravilo.
        // Ne upisuje ništa u bazu - to radi sloj koji poziva ovu metodu (REST servis / MVC kontroler)
        // preko odgovarajućeg repozitorijuma (ProizvodniNalogRepo - Entity Framework, Sloj 1).
        public RezultatPlaniranjaNaloga PripremiPlanNaloga(
            DateTime zeljeniDatumIzrade,
            IEnumerable<(int sirinaMM, int visinaMM, int kolicina)> stavke)
        {
            decimal ukupnaPovrsina = IzracunajUkupnuPovrsinu(stavke);

            if (ukupnaPovrsina <= 0)
            {
                throw new ArgumentException("Nalog mora sadržati bar jednu stavku sa validnom površinom.");
            }

            DateTime predlozeniDatum = OdrediDatumIzrade(zeljeniDatumIzrade, ukupnaPovrsina);

            return new RezultatPlaniranjaNaloga
            {
                UkupnaPovrsinaM2 = ukupnaPovrsina,
                PredlozeniDatumIzrade = predlozeniDatum,
                DatumJePomeren = predlozeniDatum.Date != zeljeniDatumIzrade.Date
            };
        }

        // Poziva se NAKON što je nalog uspešno sačuvan u bazi - "rezerviše" prostor u kapacitetu.
        public void PotvrdiRezervacijuKapaciteta(DateTime datumIzrade, decimal povrsinaNaloga)
        {
            _kapacitetRepo.AzurirajIskoriscenost(datumIzrade, povrsinaNaloga);
        }
    }

    // Pomoćna DTO klasa - rezultat primene poslovnog pravila, spreman za prikaz korisniku
    // (npr. napomena u UI-ju: "Kapacitet je popunjen, predložen je novi termin: ...")
    public class RezultatPlaniranjaNaloga
    {
        public decimal UkupnaPovrsinaM2 { get; set; }
        public DateTime PredlozeniDatumIzrade { get; set; }
        public bool DatumJePomeren { get; set; }
    }
}
