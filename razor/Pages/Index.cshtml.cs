using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Alledrogo.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _http;

    public IndexModel(IHttpClientFactory http)
    {
        _http = http;
    }

    public int ActiveAuctions { get; set; }
    public List<AuctionItem> Auctions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<HowStep> Steps { get; set; } = new();

    public string? LoggedEmail { get; set; }
    public string? LoggedRole { get; set; }

    public async Task OnGetAsync()
    {
        LoadCategories();
        LoadSteps();
        await FetchAuctionsAsync();

        LoggedEmail = HttpContext.Session.GetString("UserEmail");
        LoggedRole  = HttpContext.Session.GetString("UserRole");

        ActiveAuctions = Auctions.Count;
    }

    private async Task FetchAuctionsAsync()
    {
        try
        {
            var client = _http.CreateClient("API");

            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/auctions");
            if (!response.IsSuccessStatusCode) return;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var apiAuctions = JsonSerializer.Deserialize<List<ApiAuction>>(json, options);

            if (apiAuctions == null) return;

            Auctions = apiAuctions.Select(a => new AuctionItem
            {
                Title        = a.Name,
                Category     = a.Category,
                CategorySlug = GetSlug(a.Category),
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300",
                CurrentBid   = a.CurrentPrice > 0 ? a.CurrentPrice : a.Price,
                IsLive       = a.EndAt > DateTime.UtcNow,
                TimeLeft     = GetTimeLeft(a.EndAt),
                Id           = a.Id,
            }).ToList();
        }
        catch
        {
            // API niedostępne — zostają puste aukcje
        }
    }

    private static string GetSlug(string category) => category.ToLower() switch
    {
        "elektronika"   => "elektronika",
        "komputery"     => "komputery",
        "motoryzacja"   => "motoryzacja",
        "dom i ogród"   => "dom-ogrod",
        "budownictwo"   => "budownictwo",
        "narzędzia"     => "narzedzia",
        "rolnictwo"     => "rolnictwo",
        "sport i hobby" => "sport-hobby",
        "moda"          => "moda",
        "przemysł"      => "przemysl",
        _               => category.ToLower().Replace(" ", "-")
    };

    private static string GetTimeLeft(DateTime endAt)
    {
        var diff = endAt - DateTime.UtcNow;
        if (diff <= TimeSpan.Zero) return "Zakończona";
        if (diff.TotalDays >= 1) return $"{(int)diff.TotalDays} dni";
        if (diff.TotalHours >= 1) return $"{(int)diff.TotalHours} godz. {diff.Minutes} min";
        return $"{diff.Minutes} min";
    }

    private void LoadCategories()
    {
        Categories = new List<Category>
        {
            new() { Name = "Elektronika",   Slug = "elektronika" },
            new() { Name = "Komputery",     Slug = "komputery"   },
            new() { Name = "Motoryzacja",   Slug = "motoryzacja" },
            new() { Name = "Dom i ogród",   Slug = "dom-ogrod"   },
            new() { Name = "Budownictwo",   Slug = "budownictwo" },
            new() { Name = "Narzędzia",     Slug = "narzedzia"   },
            new() { Name = "Rolnictwo",     Slug = "rolnictwo"   },
            new() { Name = "Sport i hobby", Slug = "sport-hobby" },
            new() { Name = "Moda",          Slug = "moda"        },
            new() { Name = "Przemysł",      Slug = "przemysl"    },
        };
    }

    private void LoadSteps()
    {
        Steps = new List<HowStep>
        {
            new() { Number = 1, Title = "Zarejestruj się",  Description = "Utwórz darmowe konto w 2 minuty i uzyskaj dostęp do wszystkich aukcji." },
            new() { Number = 2, Title = "Złóż ofertę",      Description = "Licytuj na żywo." },
            new() { Number = 3, Title = "Odbierz sprzęt",   Description = "Po wygraniu aukcji opłać i odbierz przedmiot — osobiście lub wysyłką." },
        };
    }
}

public class ApiAuction
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

public class AuctionItem
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal CurrentBid { get; set; }
    public bool IsLive { get; set; }
    public long Id { get; set; }
    public string TimeLeft { get; set; } = string.Empty;
}

public class Category
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int LotCount { get; set; }
}

public class HowStep
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
