//Łączy bazę danych z kodem w C# mówi dla entityframework które klasy mają być tabelami w SQL
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Auction> Auctions { get; set; }
        public DbSet<Models.Bid> Bids { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Auction>(e =>
            {
                e.Property(x => x.Price).HasPrecision(18, 2);
                e.Property(x => x.CurrentPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Bid>(e =>
            {
                e.Property(x => x.Amount).HasPrecision(18, 2);
            });
        }

    }

}
