using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace Alledrogo.Pages;

public class IndexModel : PageModel
{
    // dummy staty
    public int ActiveAuctions { get; set; } = 120;

    public List<AuctionItem> Auctions { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<HowStep> Steps { get; set; } = new();

    public void OnGet()
    {
        LoadCategories();
        LoadAuctions();
        LoadSteps();
    }

    private void LoadAuctions()
    {
        Auctions = new List<AuctionItem>
        {
            new()
            {
                Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Elektronika", //todo fetch z bazy, for now dummy
                CategorySlug = "elektronika", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
            new()
            {
                Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Komputery.", //todo fetch z bazy, for now dummy
                CategorySlug = "komputery", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
            new()
            {
                Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Budownictwo.", //todo fetch z bazy, for now dummy
                CategorySlug = "budownictwo ", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
            new()
            {
                Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Rolnictwo.", //todo fetch z bazy, for now dummy
                CategorySlug = "rolnictwo", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
            new()
            {
                Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Moda.", //todo fetch z bazy, for now dummy
                CategorySlug = "moda", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
            new()
            {
                    Title        = "Tytuł.",  //todo fetch z bazy, for now dummy
                Category     = "Przemysł", //todo fetch z bazy, for now dummy
                CategorySlug = "przemysl", //todo fetch z bazy, for now dummy
                ImageUrl     = "https://images.unsplash.com/photo-1518791841217-8f162f1e1131?auto=format&fit=crop&w=400&h=300", //todo fetch z bazy, for now dummy
                CurrentBid   = 12_400, //todo fetch z bazy, for now dummy
                IsLive       = true, //todo fetch z bazy, for now dummy
                TimeLeft     = "2 godz. 14 min" //todo fetch z bazy, for now dummy
            },
        };
    }

    private void LoadCategories()
    {
        Categories = new List<Category>
        {
        new() { Name = "Elektronika",      Slug = "elektronika", },
        new() { Name = "Komputery",        Slug = "komputery",   },
        new() { Name = "Motoryzacja",      Slug = "motoryzacja", },
        new() { Name = "Dom i ogród",      Slug = "dom-ogrod",   },
        new() { Name = "Budownictwo",      Slug = "budownictwo", },
        new() { Name = "Narzędzia",        Slug = "narzedzia",   },
        new() { Name = "Rolnictwo",        Slug = "rolnictwo",   },
        new() { Name = "Sport i hobby",    Slug = "sport-hobby", },
        new() { Name = "Moda",             Slug = "moda",        },
        new() { Name = "Przemysł",         Slug = "przemysl",    },
        };
    }

    private void LoadSteps()
    {
        Steps = new List<HowStep>
        {
            new() { Number = 1, Title = "Zarejestruj się",   Description = "Utwórz darmowe konto w 2 minuty i uzyskaj dostęp do wszystkich aukcji." },
            new() { Number = 2, Title = "Złóż ofertę",       Description = "Licytuj na żywo" },
            new() { Number = 3, Title = "Odbierz sprzęt",    Description = "Po wygraniu aukcji opłać i odbierz przedmiot — osobiście lub wysyłką." },
        };
    }
}


public class AuctionItem
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal CurrentBid { get; set; }
    public bool IsLive { get; set; }
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
