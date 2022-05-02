using Microsoft.EntityFrameworkCore;

namespace BvsDesktopLinux.Models
{
    public class BanknotesDbContext : DbContext
    {
        public DbSet<Banknote> Banknotes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=banknotes.db");
        }
    }
}
