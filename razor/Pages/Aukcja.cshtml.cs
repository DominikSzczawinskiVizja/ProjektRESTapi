using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class AukcjaModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public AukcjaModel(IHttpClientFactory http)
        {
            _http = http;
        }

        public AukcjaDetails? Aukcja { get; set; }
        public string? ErrorMessage { get; set; }
        public string? BidError { get; set; }
        public string? BidSuccess { get; set; }
        public string? LoggedEmail { get; set; }

        [BindProperty]
        public decimal Kwota { get; set; }

        public async Task<IActionResult> OnGetAsync(long id)
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            await FetchAukcja(id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long id)
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");

            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var payload = new { Amount = Kwota };
                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync($"api/bids/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var err = JsonSerializer.Deserialize<AukcjaErrorResponse>(errorJson, options);
                        BidError = err?.Error ?? "Nie udało się złożyć oferty.";
                    }
                    catch
                    {
                        BidError = "Nie udało się złożyć oferty.";
                    }
                }
                else
                {
                    BidSuccess = $"Oferta {Kwota:N0} zł została złożona!";
                }
            }
            catch
            {
                BidError = "Nie można połączyć się z serwerem.";
            }

            await FetchAukcja(id);
            return Page();
        }

        private async Task FetchAukcja(long id)
        {
            try
            {
                var client = _http.CreateClient("API");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var token = HttpContext.Session.GetString("AccessToken");
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/auctions/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Nie znaleziono aukcji.";
                    return;
                }

                var json = await response.Content.ReadAsStringAsync();
                Aukcja = JsonSerializer.Deserialize<AukcjaDetails>(json, options);
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem.";
            }
        }
    }

    public class AukcjaDetails
    {
        public long Id { get; set; }
        public long OnwerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal PriceNow { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AukcjaErrorResponse
    {
        public string? Error { get; set; }
    }
}
