using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace razor.Pages
{
    public class OfertaModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public OfertaModel(IHttpClientFactory http)
        {
            _http = http;
        }

        public string? LoggedEmail { get; set; }
        public string? ErrorMessage { get; set; }
        public OfertaDto? Oferta { get; set; }

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

                var response = await client.GetAsync($"api/bids/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Nie znaleziono oferty.";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                Oferta = JsonSerializer.Deserialize<OfertaDto>(json, options);
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem.";
            }

            return Page();
        }
    }

    public class OfertaDto
    {
        public long Id { get; set; }
        public decimal Amount { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string? UserMiddleName { get; set; }
        public string UserLastName { get; set; } = string.Empty;
        public string AuctionName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public long AuctionId { get; set; }
        public long UserId { get; set; }
    }
}
