using BvsDesktopLinux.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BvsDesktopLinux.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Контейнер используется в XAML как источник данных DataGrid
        public ObservableCollection<Banknote> Banknotes { get; }

        public MainWindowViewModel()
        {
            BanknotesDbContext _dbContext = new();

            // Убеждаемся в том, что база данных существует. Если её не будет, метод
            // EnsureCreated() создаст схему
            _dbContext.Database.EnsureCreated();

            // Выполняем Seed Data - заполняем базу данных минимально необходимыми данными
            // для обеспечения возможности отладки приложения
            var _banknotes = _dbContext.Banknotes.FirstOrDefault(b => b.Id == 1);
            if (null == _banknotes)
            {
                var banknotes = new List<Banknote> {
                    new Banknote { Id = 1, Currency = "RUB", Denomination = "10" },
                    new Banknote { Id = 2, Currency = "RUB", Denomination = "50" },
                    new Banknote { Id = 3, Currency = "RUB", Denomination = "100" }
                };

                banknotes.ForEach(s => _dbContext.Banknotes.Add(s));
                _dbContext.SaveChanges();
            }

            // Добавляем в контейнер ObservableCollection<Banknote> все записи
            // из таблицы Banknotes базы данных
            Banknotes = new ObservableCollection<Banknote>(_dbContext.Banknotes);
        }
    }
}
