﻿using Avalonia.Metadata;
using BvsDesktopLinux.Models;
using ReactiveUI;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Npgsql.PostgresTypes;    // Нужен в случае использования Migrate()

namespace BvsDesktopLinux.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Контейнер используется в XAML как источник данных DataGrid
        public ObservableCollection<Banknote> Banknotes { get; }

        // Для того, чтобы связать событие изменения SelectedBanknote, осуществляемое
        // через DataGrid и метод CanUpdate() кнопки "Delete Item", необходимо создать
        // дополнительной свойство IsBanknoteSelected, через которое и передаётся
        // информация об изменении выбранного элемента
        private Boolean isBanknoteSelected = false;

        public Boolean IsBanknoteSelected
        {
            get { return isBanknoteSelected; }
            set
            {
                this.RaiseAndSetIfChanged(ref isBanknoteSelected, value);
            }
        }

        // Выбранная пользователем банкнота
        private Banknote? selectedBanknote = null;

        public Banknote? SelectedBanknote {
            get { return selectedBanknote; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedBanknote, value);

                // Информируем подписчиков (метод CanDeleteBanknote) об изменении состояния
                // SelectedBanknote, косвенным способом через изменение свойства IsBanknoteSelected
                IsBanknoteSelected = (null != selectedBanknote);
            } 
        }

        // Сообщение о ошибке подключения к базе данных
        private String dbAccessFailure = String.Empty;

        public String DbAccessFailure
        {
            get { return dbAccessFailure; }
            set
            {
                this.RaiseAndSetIfChanged(ref dbAccessFailure, value);
            }
        }

        // Свойство, определяющее видимость главного окна
        private bool uiVisible = true;

        public bool UiVisible
        {
            get { return uiVisible; }
            set
            {
                this.RaiseAndSetIfChanged(ref uiVisible, value);
            }
        }

        // Свойство, определяющее видимость блока с описанием ошибки подключения
        // к базе данных
        private bool errorBlockVisible = false;

        public bool ErrorBlockVisible
        {
            get { return errorBlockVisible; }
            set
            {
                this.RaiseAndSetIfChanged(ref errorBlockVisible, value);
            }
        }

        public MainWindowViewModel()
        {
            BanknotesDbContext _dbContext = new();

            // Убеждаемся в том, что база данных существует. Если её не будет, метод
            // EnsureCreated() создаст схему. Если же база создана, но схема не сформирована,
            // метод EnsureCreated() не создаст ничего. В этом случае следует использовать
            // метод Migrate()
            //
            // Метод проверяет, была ли создана база данных. Если базы не было, то вызов
            // создаст её и вернёт true. Если база есть, то метод EnsureCreated() вернёт
            // false.
            // Мы анализуем код возврата EnsureCreated() и если база есть, анализируем
            // необходимо ли применять к ней миграции.
            // Оптимальным поведением было бы выполнять миграцию на новую базу данных
            // при установке обновления программного обеспечения
            try
            {
                if (!_dbContext.Database.EnsureCreated())
                {
                    var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                    if (pendingMigrations.Any())
                    {
                        try
                        {
                            _dbContext.Database.Migrate();  // Определён в Microsoft.EntityFrameworkCore;
                        }
                        catch (Exception ex) {
                            // Это исключение может срабатывать каждый раз, т.к. в проекте есть
                            // несколько миграций и метод GetPendingMigrations() именно об этом
                            // и сообщает.
                            // TODO: нужно проверять, применены ли миграции, или нет
                            Console.WriteLine(ex);
                        }
                    }
                }
            }
            catch (Npgsql.NpgsqlException ex)
            {
                // Не удалось подключиться к базе данных.
                Banknotes = new ObservableCollection<Banknote>();

                // Сохраняем информацию об ошибке подключения во внутренней переменной
                DbAccessFailure = ex.Message;
                uiVisible = false;
                ErrorBlockVisible = true;
                return;
            }

            // Выполняем Seed Data - заполняем базу данных минимально необходимыми данными
            // для обеспечения возможности отладки приложения
            var _banknotes = _dbContext.Banknotes.FirstOrDefault(b => b.Id == 1);
            if (null == _banknotes)
            {
                var banknotes = new List<Banknote> {
                    new() { Id = 1, Currency = "RUB", Denomination = "10", Status="" },
                    new() { Id = 2, Currency = "RUB", Denomination = "50", Status="" },
                    new() { Id = 3, Currency = "RUB", Denomination = "1000", Status="Rejected" },
                    new() { Id = 4, Currency = "RUB", Denomination = "200", Status="" },
                    new() { Id = 5, Currency = "RUB", Denomination = "0", Status="Jammed" }
                };

                banknotes.ForEach(s => _dbContext.Banknotes.Add(s));
                _dbContext.SaveChanges();
            }

            // Добавляем в контейнер ObservableCollection<Banknote> все записи
            // из таблицы Banknotes базы данных
            Banknotes = new ObservableCollection<Banknote>(_dbContext.Banknotes);
        }

        // Обработчик нажатия на кнопку в пользовательском интерфейсе приложения
        public int RestoreCounts()
        {
            string currency = "RUB";

            // Сбрасываем старые пересчёты
            Banknotes.Clear();

            // Добавляем в список несколько описаний "принятых" купюр для имитации взноса
            Banknotes.Add(new() { Id = 6, Currency = currency, Denomination = "200", Status = "" });
            Banknotes.Add(new() { Id = 7, Currency = currency, Denomination = "100", Status = "Rejected" });
            Banknotes.Add(new() { Id = 8, Currency = currency, Denomination = "2000", Status = "" });
            Banknotes.Add(new() { Id = 9, Currency = currency, Denomination = "5000", Status = "" });

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

            // Не обязательно, на разумно: добавить мета-данные в PDF-документ
            var metadata = new SKDocumentPdfMetadata
            {
                Author = "Maskim Rozhkov",
                Creation = DateTime.Now,
                Creator = "Maskim Rozhkov with SkiaSharp",
                Keywords = "SkiaSharp, PDF, Developer, Library",
                Modified = DateTime.Now,
                Producer = "SkiaSharp",
                Subject = "SkiaSharp Sample PDF",
                Title = "Sample PDF",
            };

            using var doc = SKDocument.CreatePdf("SkiaSample.pdf", metadata);

            // Критичное упрощение: все записи выводятся только на одну страницу,
            // размер страницы фиксированный
            float pageWidth = 840.0f;
            float pageHeight = 1188.0f;

            // Получаем контекст вывода данных (контекст отрисовки)
            using (var pdfCanvas = doc.BeginPage(pageWidth, pageHeight))
            {
                // Формируем структуру, которая описывает параметры отображения элемента
                using var paint = new SKPaint
                {
                    TextSize = 48.0f,
                    IsAntialias = true,
                    Color = SKColors.Black,
                    IsStroke = true,
                    StrokeWidth = 2,
                    TextAlign = SKTextAlign.Left
                };

                float curPos = 0.0f;    // Текущая позиция вывода - изменяется на каждой итерации
                float margin = 24.0f;   // Отступы к каждой из сторон
                float oneThird = (pageWidth - margin * 2) / 3;  // Каждая колонка шириной в треть

                foreach (var note in Banknotes)
                {
                    pdfCanvas.DrawText(note.Id.ToString(), margin, curPos + paint.TextSize, paint);
                    pdfCanvas.DrawText(note.Currency, margin + oneThird, curPos + paint.TextSize, paint);
                    pdfCanvas.DrawText(note.Denomination, margin + oneThird * 2, curPos + paint.TextSize, paint);
                    curPos += paint.TextSize;
                }

                doc.EndPage();
            }

            doc.Close();
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
