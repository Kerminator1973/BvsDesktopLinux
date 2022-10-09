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
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var connectionString = config.GetSection("ConnectionStrings").GetSection("psql").Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlite("Data Source=banknotes.db");
            }
            else
            {
                // https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL/6.0.7
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
    }
}
