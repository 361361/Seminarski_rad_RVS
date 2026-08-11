using System.Net.Http.Json;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Servisi
{
    // Ova klasa je "most" između prezentacionog sloja i sloja servisa (REST API) -
    // pretvara pozive kontrolera u HTTP zahteve ka RVS_REST_API.
    public class NaloziApiServis
    {
        private readonly HttpClient _httpKlijent;

        public NaloziApiServis(IHttpClientFactory httpKlijentFabrika)
        {
            _httpKlijent = httpKlijentFabrika.CreateClient("RVS_REST_API");
        }

        public async Task<List<NalogPrikazViewModel>> DajSveAsync(string? filter = null)
        {
            string ruta = string.IsNullOrWhiteSpace(filter)
                ? "api/NaloziApi"
                : $"api/NaloziApi?filter={Uri.EscapeDataString(filter)}";

            var rezultat = await _httpKlijent.GetFromJsonAsync<List<NalogPrikazViewModel>>(ruta);
            return rezultat ?? new List<NalogPrikazViewModel>();
        }

        public async Task<NalogPrikazViewModel?> DajPoIdAsync(int id)
        {
            var odgovor = await _httpKlijent.GetAsync($"api/NaloziApi/{id}");
            if (!odgovor.IsSuccessStatusCode) return null;
            return await odgovor.Content.ReadFromJsonAsync<NalogPrikazViewModel>();
        }

        public async Task<NalogKreiranOdgovor?> KreirajAsync(NoviNalogViewModel model)
        {
            var telo = new
            {
                model.IdKupac,
                model.IdKorisnik,
                model.ZeljeniDatumIzrade,
                Stavke = model.Stavke.Select(s => new
                {
                    s.TipElementa,
                    s.SirinaMM,
                    s.VisinaMM,
                    s.Kolicina,
                    s.BojaProfila,
                    s.TipStakla,
                    s.TipOkova
                })
            };

            var odgovor = await _httpKlijent.PostAsJsonAsync("api/NaloziApi", telo);
            if (!odgovor.IsSuccessStatusCode) return null;

            return await odgovor.Content.ReadFromJsonAsync<NalogKreiranOdgovor>();
        }

        public async Task<bool> IzmeniStatusAsync(int id, string noviStatus)
        {
            var odgovor = await _httpKlijent.PutAsJsonAsync($"api/NaloziApi/{id}/status", noviStatus);
            return odgovor.IsSuccessStatusCode;
        }

        public async Task<bool> ObrisiAsync(int id)
        {
            var odgovor = await _httpKlijent.DeleteAsync($"api/NaloziApi/{id}");
            return odgovor.IsSuccessStatusCode;
        }
    }
}
