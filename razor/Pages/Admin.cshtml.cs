using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace razor.Pages
{
    public class AdminModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public AdminModel(IHttpClientFactory http)
        {
            _http = http;
        }

        public string? LoggedEmail { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public List<AdminUserDto> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            LoggedEmail = HttpContext.Session.GetString("UserEmail");
            var token = HttpContext.Session.GetString("AccessToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token)) return RedirectToPage("/add_user");
            if (role != "Admin")
            {
                ErrorMessage = "Brak dostępu. Ta strona jest tylko dla administratorów.";
                return Page();
            }

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var response = await client.GetAsync("api/users");
                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Nie udało się pobrać listy użytkowników.";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                Users = JsonSerializer.Deserialize<List<AdminUserDto>>(json, options) ?? new();
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
            var role = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(token) || role != "Admin") return RedirectToPage("/add_user");

            try
            {
                var client = _http.CreateClient("API");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                await client.DeleteAsync($"api/users/{id}");
            }
            catch { }

            return RedirectToPage("/Admin");
        }
    }

    public class AdminUserDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
