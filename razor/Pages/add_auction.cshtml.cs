using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace razor.Pages
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class add_auctionModel : PageModel
    {
        [BindProperty]
        public string? Nazwa { get; set; }

        [BindProperty]
        public decimal Cenawywoławcza { get; set; }
       
        [BindProperty]
        public string? Kategoria { get; set; }

        [BindProperty]
        public int Czasaukcji { get; set; } = 24; //podstawowo jest 24h

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
