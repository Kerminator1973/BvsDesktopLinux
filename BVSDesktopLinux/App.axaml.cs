using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BvsDesktopLinux.ViewModels;
using BvsDesktopLinux.Views;
using System;
using System.Threading;

namespace BvsDesktopLinux
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            // Здесь можно выполнить регистрацию сервисов Dependency Injection
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Обрабатываем параметры командной строки с целью явным образом
                // указать локализацию. Это особенно удобно для того, чтобы выполнять
                // тестирование и отладку локализованных сообщений
                ParseCommandLine(Environment.GetCommandLineArgs());

                // Создаём основное окно
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            // Инициализация приложения завершена, можно визуализировать
            // пользовательский интерфейс
            base.OnFrameworkInitializationCompleted();
        }

        protected void ParseCommandLine(string[] commandLine)
        {
            bool bIsLocale = false;

            foreach (var elem in commandLine)
            {
                if (elem.StartsWith("-"))
                {
                    // Параметр locale позволяет установить язык, с которым должно работать приложение
                    int comparison = String.Compare(elem, "-locale", comparisonType: StringComparison.OrdinalIgnoreCase);
                    if (comparison == 0)
                    {
                        bIsLocale = true;
                    }
                }
                else
                {
                    if (bIsLocale)
                    {
                        bIsLocale = false;

                        var culture = new System.Globalization.CultureInfo(elem);

                        // Locale можно задать так: "ru-RU", "en"
                        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

                        // Чтобы в ModelView можно было загружать локализованные сообщения,
                        // необходимо установить значение не только CurrentUICulture,
                        // но и CurrentCulture
                        Thread.CurrentThread.CurrentCulture = culture;
                    }
                }
            }
        }
    }
}