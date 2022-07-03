using Avalonia.Metadata;
using BvsDesktopLinux.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace BvsDesktopLinux.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        // Контейнер используется в XAML как источник данных DataGrid
        public ObservableCollection<Banknote> Banknotes { get; }

        // Выбранная пользователем банкнота
        private Banknote? selectedBanknote = null;

        // Для того, чтобы связать событие изменения SelectedBanknote, осуществляемое
        // через DataGrid и метод CanUpdate() кнопки "Delete Item", необходимо выполнить
        // ряд дополнительных действий вручную
        public event PropertyChangedEventHandler? PropertyChanged;

        bool IsBanknoteSelected = false;
        public Banknote? SelectedBanknote {
            get { return selectedBanknote; }
            set
            {
                if (value == selectedBanknote)
                    return;
                selectedBanknote = value;

                // Информируем подписчиков (метод CanDeleteBanknote) об изменении состояния
                // SelectedBanknote, косвенным способом через изменение свойства IsBanknoteSelected
                IsBanknoteSelected = (null != selectedBanknote);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBanknoteSelected)));
            } 
        }

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

        // Обработчик нажатия на кнопку в пользовательском интерфейсе приложения
        public int RestoreCounts(string currency)
        {
            // Сбрасываем старые пересчёты
            Banknotes.Clear();

            // Добавляем в список несколько описаний "принятых" купюр для имитации взноса
            Banknotes.Add(new Banknote { Id = 4, Currency = currency, Denomination = "200" });
            Banknotes.Add(new Banknote { Id = 5, Currency = currency, Denomination = "100" });
            Banknotes.Add(new Banknote { Id = 6, Currency = currency, Denomination = "2000" });
            Banknotes.Add(new Banknote { Id = 7, Currency = currency, Denomination = "5000" });

            return Banknotes.Count;
        }

        public void PrintReport()
        {
            // В Avalonia нет класса PrintDialog и FlowDocument. По этой причине, необходимо
            // использовать другие подходы для реализации печати, или экспорта в PDF
            //
            // https://github.com/Oaz/AvaloniaUI.PrintToPDF
            // https://github.com/AvaloniaUI/Avalonia/blob/master/src/Avalonia.Diagnostics/Diagnostics/VisualExtensions.cs#L56
            // https://github.com/jp2masa/Movere
        }

        public void DeleteBanknote()
        {
            if (null != SelectedBanknote)
            {
                Banknotes.Remove(SelectedBanknote);
            }
        }

        // Метод CanDeleteBanknote() будет вызываться если измениться свойство IsBanknoteSelected
        [DependsOn(nameof(IsBanknoteSelected))]
        public bool CanDeleteBanknote(/* CommandParameter */object parameter)
        {
            return null != SelectedBanknote;
        }
    }
}
