//Łączy bazę danych z kodem w C# mówi dla entityframework które klasy mają być tabelami w SQL
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Auction> Auctions { get; set; }
        public DbSet<Models.Bid> Bids { get; set; } = null!;

    }
}
