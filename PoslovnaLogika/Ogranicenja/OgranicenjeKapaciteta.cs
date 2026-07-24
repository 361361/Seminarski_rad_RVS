using System.Text.Json;

namespace PoslovnaLogika.Klase
{
    // Klasa čita parametar poslovnog pravila (maksimalni dnevni kapacitet, K)
    // iz eksternog JSON fajla - u skladu sa zahtevom da uslov/posledica pravila
    // budu parametrizovani, a ne "ukucani" (hardkodovani) u kodu.
    //
    // Napomena o putanji: koristi se AppContext.BaseDirectory (fizička putanja bin
    // foldera procesa koji se izvršava - REST API ili MVC aplikacija) umesto proste
    // relativne putanje, da bi čitanje JSON-a pouzdano radilo bez obzira iz kog
    // direktorijuma je aplikacija pokrenuta.
    public class OgranicenjeKapaciteta
    {
        private static readonly string PodrazumevanaPutanja =
            Path.Combine(AppContext.BaseDirectory, "Ogranicenja", "ogranicenjeKapaciteta.json");

        public decimal UzmiMaksimalniKapacitetIzJSON(string? putanja = null)
        {
            string stvarnaPutanja = putanja ?? PodrazumevanaPutanja;

            if (!File.Exists(stvarnaPutanja))
            {
                throw new FileNotFoundException(
                    $"JSON fajl sa ograničenjem kapaciteta nije pronađen na putanji: {stvarnaPutanja}. " +
                    "Proveri da li je 'Ogranicenja/ogranicenjeKapaciteta.json' postavljen da se kopira " +
                    "u izlazni (bin) folder projekta koji se pokreće (Copy to Output Directory).");
            }

            string json = File.ReadAllText(stvarnaPutanja);
            var podaci = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);

            if (podaci == null || !podaci.ContainsKey("MaxKapacitetM2"))
            {
                throw new InvalidDataException("Parametar 'MaxKapacitetM2' nije definisan u JSON fajlu.");
            }

            return podaci["MaxKapacitetM2"];
        }
    }
}
