using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class edytuj_aukcjeModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public edytuj_aukcjeModel(IHttpClientFactory http)
        {
            _http = http;
        }

        public string? LoggedEmail { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        [BindProperty] public string? Nazwa { get; set; }
        [BindProperty] public string? Opis { get; set; }
        [BindProperty] public string? Kategoria { get; set; }
        [BindProperty] public decimal Cena { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var response = await client.GetAsync($"api/auctions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Nie znaleziono aukcji.";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var aukcja = JsonSerializer.Deserialize<AukcjaEditDto>(json, options);
                if (aukcja != null)
                {
                    Nazwa = aukcja.Name;
                    Opis = aukcja.Description;
                    Kategoria = aukcja.Category;
                    Cena = aukcja.Price;
                }
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var payload = new { Name = Nazwa, Description = Opis, Category = Kategoria, Price = Cena };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/auctions/{id}", content);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    try { ErrorMessage = JsonSerializer.Deserialize<ApiError>(err, options)?.Error ?? "Nie udało się zapisać zmian."; }
                    catch { ErrorMessage = "Nie udało się zapisać zmian."; }
                }
                else
                {
                    SuccessMessage = "Aukcja została zaktualizowana.";
                }
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(long id)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await client.DeleteAsync($"api/auctions/{id}");
            }
            catch { }

            return RedirectToPage("/Index");
        }
    }

    public class AukcjaEditDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class ApiError
    {
        public string? Error { get; set; }
    }
}
