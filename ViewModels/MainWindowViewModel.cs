using BvsDesktopLinux.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BvsDesktopLinux.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Banknote> Banknotes { get; }

        public MainWindowViewModel()
        {
#if __RUN_WITHOUT_DATABASE
            Banknotes = new ObservableCollection<Banknote>(GenerateMockBanknotesTable());
#else
            BanknotesDbContext _dbContext = new();
            Banknotes = new ObservableCollection<Banknote>(_dbContext.Banknotes);
#endif
        }

        private IEnumerable<Banknote> GenerateMockBanknotesTable()
        {
            var defaultBanknotes = new List<Banknote>()
            {
                new Banknote { Id = 1, Currency = "RUB", Denomination = "10" },
                new Banknote { Id = 2, Currency = "RUB", Denomination = "50" },
                new Banknote { Id = 3, Currency = "RUB", Denomination = "100" }
            };

            return defaultBanknotes;
        }
    }
}
