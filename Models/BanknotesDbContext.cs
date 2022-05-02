using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BvsDesktopLinux.Models
{
    public class BanknotesDbContext : DbContext
    {
        public DbSet<Banknote> Banknotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=banknotes.db");
        }
    }
}
