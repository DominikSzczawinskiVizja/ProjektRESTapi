using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class add_userModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public add_userModel(IHttpClientFactory http)
        {
            _http = http;
        }

        [BindProperty]
        [Required(ErrorMessage = "E-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail")]
        public string? Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Haslo { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var client = _http.CreateClient("API");
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var payload = new { Email, Password = Haslo };
                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Błędny E-mail lub Hasło";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(json, options);

                if (result == null)
                {
                    ErrorMessage = "Błąd serwera. Spróbuj ponownie.";
                    return Page();
                }

                HttpContext.Session.SetString("AccessToken", result.AccessToken);
                HttpContext.Session.SetString("UserEmail", result.Email);
                HttpContext.Session.SetString("UserRole", result.Role);
                HttpContext.Session.SetString("UserId", result.UserId.ToString());

                return RedirectToPage("/Index");
            }
            catch
            {
                ErrorMessage = "Nie można połączyć się z serwerem. Spróbuj ponownie później.";
                return Page();
            }
        }
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public long UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string? Error { get; set; }
    }
}