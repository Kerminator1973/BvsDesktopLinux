using BvsDesktopLinux.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BvsDesktopLinux.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Banknote> Banknotes { get; }

        public MainWindowViewModel()
        {
            Banknotes = new ObservableCollection<Banknote>(GenerateMockBanknotesTable());
        }

        private IEnumerable<Banknote> GenerateMockBanknotesTable()
        {
            var defaultBanknotes = new List<Banknote>()
            {
                new Banknote()
                {
                    Currency = "RUB",
                    Denomination = "10"
                },
                new Banknote()
                {
                    Currency = "RUB",
                    Denomination = "50"
                },
                new Banknote()
                {
                    Currency = "RUB",
                    Denomination = "100"
                }
            };

            return defaultBanknotes;
        }
    }
}
