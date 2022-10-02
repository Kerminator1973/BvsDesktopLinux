using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BvsDesktopLinux.Models
{
    public class BanknotesDbContext : DbContext
    {
        public DbSet<Banknote> Banknotes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
/*
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);

            var configuration = builder.Build();
            var name = configuration.GetSection("ConnectionStrings").GetSection("sqlite").Value;
*/
            optionsBuilder.UseSqlite("Data Source=banknotes.db");

            // https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/6.0.7
            // optionsBuilder.UseNpgsql(@"Host=127.0.0.1;Username=developer;Password=38Gjgeuftd;Database=test");
        }
    }
}
