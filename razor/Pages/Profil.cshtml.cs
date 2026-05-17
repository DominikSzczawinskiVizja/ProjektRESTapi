using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class ProfilModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public ProfilModel(IHttpClientFactory http)
        {
            _http = http;
        }

        public string? LoggedEmail { get; set; }
        public string? LoggedRole { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public List<ProfilAukcjaDto> MojeAukcje { get; set; } = new();

        [BindProperty] public string? Imie { get; set; }
        [BindProperty] public string? Nazwisko { get; set; }
        [BindProperty] public string? DrugieImie { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? Haslo { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            LoggedRole  = HttpContext.Session.GetString("UserRole");
            var token   = HttpContext.Session.GetString("AccessToken");
            var userId  = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            var client  = _http.CreateClient("API");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var response = await client.GetAsync($"api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<ProfilUserDto>(json, options);
                    if (user != null)
                    {
                        Imie       = user.FirstName;
                        Nazwisko   = user.LastName;
                        DrugieImie = user.MiddleName;
                        Email      = user.Email;
                    }
                }
            }
            catch { }

            try
            {
                var response = await client.GetAsync("api/auctions");
                if (response.IsSuccessStatusCode)
                {
                    var json   = await response.Content.ReadAsStringAsync();
                    var aukcje = JsonSerializer.Deserialize<List<ProfilAukcjaDto>>(json, options) ?? new();
                    var myId   = long.TryParse(userId, out var pid) ? pid : 0;
                    MojeAukcje = aukcje.Where(a => a.OwnerId == myId).ToList();
                }
            }
            catch { }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            LoggedRole  = HttpContext.Session.GetString("UserRole");
            var token   = HttpContext.Session.GetString("AccessToken");
            var userId  = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            try
            {
                var client  = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var payload = new
                {
                    FirstName  = Imie,
                    LastName   = Nazwisko,
                    MiddleName = DrugieImie,
                    Email      = Email,
                    Password   = string.IsNullOrWhiteSpace(Haslo) ? null : Haslo
                };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/users/{userId}", content);
                if (!response.IsSuccessStatusCode)
                {
                    var err = await response.Content.ReadAsStringAsync();
                    try { ErrorMessage = JsonSerializer.Deserialize<ApiError>(err, options)?.Error ?? "Nie udało się zapisać zmian."; }
                    catch { ErrorMessage = "Nie udało się zapisać zmian."; }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Email))
                        HttpContext.Session.SetString("UserEmail", Email);
                    SuccessMessage = "Dane zostały zaktualizowane.";
                    LoggedEmail    = HttpContext.Session.GetString("UserEmail");
                }

                var aukcjeResp = await client.GetAsync("api/auctions");
                if (aukcjeResp.IsSuccessStatusCode)
                {
                    var json   = await aukcjeResp.Content.ReadAsStringAsync();
                    var aukcje = JsonSerializer.Deserialize<List<ProfilAukcjaDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    var myId   = long.TryParse(userId, out var pid) ? pid : 0;
                    MojeAukcje = aukcje.Where(a => a.OwnerId == myId).ToList();
                }
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var token  = HttpContext.Session.GetString("AccessToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await client.DeleteAsync($"api/users/{userId}");
            }
            catch { }

            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }

    public class ProfilUserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ProfilAukcjaDto
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal PriceNow { get; set; }
        public DateTime EndAt { get; set; }
    }
}
