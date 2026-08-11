using System.Net.Http.Json;
using RVS_Aplikacija.ViewModels;

namespace RVS_Aplikacija.Servisi
{
    public class KupacApiServis
    {
        private readonly HttpClient _httpKlijent;

        public KupacApiServis(IHttpClientFactory httpKlijentFabrika)
        {
            _httpKlijent = httpKlijentFabrika.CreateClient("RVS_REST_API");
        }

        public async Task<List<KupacViewModel>> DajSveAsync(string? filter = null)
        {
            string ruta = string.IsNullOrWhiteSpace(filter)
                ? "api/KupacApi"
                : $"api/KupacApi?filter={Uri.EscapeDataString(filter)}";

            var rezultat = await _httpKlijent.GetFromJsonAsync<List<KupacViewModel>>(ruta);
            return rezultat ?? new List<KupacViewModel>();
        }

        public async Task<KupacViewModel?> DajPoIdAsync(int id)
        {
            var odgovor = await _httpKlijent.GetAsync($"api/KupacApi/{id}");
            if (!odgovor.IsSuccessStatusCode) return null;
            return await odgovor.Content.ReadFromJsonAsync<KupacViewModel>();
        }

        public async Task<bool> DodajAsync(KupacViewModel model)
        {
            var odgovor = await _httpKlijent.PostAsJsonAsync("api/KupacApi", model);
            return odgovor.IsSuccessStatusCode;
        }

        public async Task<bool> IzmeniAsync(KupacViewModel model)
        {
            var odgovor = await _httpKlijent.PutAsJsonAsync($"api/KupacApi/{model.IdKupac}", model);
            return odgovor.IsSuccessStatusCode;
        }

        public async Task<bool> ObrisiAsync(int id)
        {
            var odgovor = await _httpKlijent.DeleteAsync($"api/KupacApi/{id}");
            return odgovor.IsSuccessStatusCode;
        }
    }
}
