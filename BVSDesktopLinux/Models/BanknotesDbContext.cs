using Microsoft.EntityFrameworkCore;

namespace BvsDesktopLinux.Models
{
    public class BanknotesDbContext : DbContext
    {
        public DbSet<Banknote> Banknotes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=banknotes.db");

            // https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/6.0.7
            // optionsBuilder.UseNpgsql(@"Host=127.0.0.1;Username=developer;Password=38Gjgeuftd;Database=test");
        }
    }
}
