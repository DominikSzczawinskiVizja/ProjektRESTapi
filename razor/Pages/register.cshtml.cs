using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace razor.Pages
{
    public class registerModel : PageModel
    {
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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            return RedirectToPage("/Index");
        }
    }
}
