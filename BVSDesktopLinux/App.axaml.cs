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

            // ����� ����� ��������� ����������� �������� Dependency Injection
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // ������������ ��������� ��������� ������ � ����� ����� �������
                // ������� �����������. ��� �������� ������ ��� ����, ����� ���������
                // ������������ � ������� �������������� ���������
                ParseCommandLine(Environment.GetCommandLineArgs());

                // ������ �������� ����
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }

            // ������������� ���������� ���������, ����� ���������������
            // ���������������� ���������
            base.OnFrameworkInitializationCompleted();
        }

        protected void ParseCommandLine(string[] commandLine)
        {
            bool bIsLocale = false;

            foreach (var elem in commandLine)
            {
                if (elem.StartsWith("-"))
                {
                    // �������� locale ��������� ���������� ����, � ������� ������ �������� ����������
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

                        // Locale ����� ������ ���: "ru-RU", "en"
                        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

                        // ����� � ModelView ����� ���� ��������� �������������� ���������,
                        // ���������� ���������� �������� �� ������ CurrentUICulture,
                        // �� � CurrentCulture
                        Thread.CurrentThread.CurrentCulture = culture;
                    }
                }
            }
        }
    }
}