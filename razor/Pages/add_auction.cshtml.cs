using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class add_auctionModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public add_auctionModel(IHttpClientFactory http)
        {
            _http = http;
        }

        [BindProperty]
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Nazwa { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Opis jest wymagany")]
        public string? Opis { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Kategoria jest wymagana")]
        public string? Kategoria { get; set; }

        [BindProperty]
        public decimal Cenawywoławcza { get; set; }

        [BindProperty]
        public int Czasaukcji { get; set; } = 24;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // jeśli nie zalogowany — przekieruj do logowania
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AccessToken")))
                Response.Redirect("/add_user");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var token = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var payload = new
                {
                    Name        = Nazwa,
                    Description = Opis,
                    Category    = Kategoria,
                    Price       = Cenawywoławcza,
                    EndAt       = DateTime.UtcNow.AddHours(Czasaukcji)
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("api/auctions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var err = JsonSerializer.Deserialize<AuctionErrorResponse>(errorJson, options);
                        ErrorMessage = err?.Error ?? "Nie udało się wystawić aukcji.";
                    }
                    catch
                    {
                        ErrorMessage = "Nie udało się wystawić aukcji.";
                    }
                    return Page();
                }

                return RedirectToPage("/Index");
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem. Spróbuj ponownie później.";
                return Page();
            }
        }
    }

    public class AuctionErrorResponse
    {
        public string? Error { get; set; }
    }
}
