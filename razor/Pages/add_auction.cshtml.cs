using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace razor.Pages
{
    public class add_auctionModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Nazwa { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Kategoria jest wymagana")]
        public string? Kategoria { get; set; }

        [BindProperty]
        public decimal Cenawywoławcza { get; set; }

        [BindProperty]
        public int Czasaukcji { get; set; } = 24;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var endDate = DateTime.Now.AddHours(Czasaukcji);

            return RedirectToPage("/Index");
        }
    }
}
