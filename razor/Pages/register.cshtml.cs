using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace razor.Pages
{
    public class registerModel : PageModel
    {
        private readonly IHttpClientFactory _http;

        public registerModel(IHttpClientFactory http)
        {
            _http = http;
        }

        [BindProperty]
        [Required(ErrorMessage = "Imię jest wymagane")]
        public string? Imie { get; set; }

        [BindProperty]
        public string? DrugieImie { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string? Nazwisko { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "E-mail jest wymagany")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres e-mail")]
        public string? Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(8, ErrorMessage = "Hasło musi mieć minimum 8 znaków")]
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

                var payload = new
                {
                    FirstName = Imie,
                    LastName = Nazwisko,
                    Email,
                    Password = Haslo
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("api/auth/register", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorJson = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Status: {response.StatusCode} | Odpowiedź: {errorJson}";
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<AuthResponse>(json, options);

                if (result != null)
                {
                    HttpContext.Session.SetString("AccessToken", result.AccessToken);
                    HttpContext.Session.SetString("UserEmail", result.Email);
                    HttpContext.Session.SetString("UserRole", result.Role);
                    HttpContext.Session.SetString("UserId", result.UserId.ToString());
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
}