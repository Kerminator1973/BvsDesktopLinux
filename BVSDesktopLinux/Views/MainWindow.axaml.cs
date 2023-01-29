using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Xaml.Interactions.Core;

namespace BvsDesktopLinux.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_OnLoadingRow(object? sender, DataGridRowEventArgs e)
        {
            // Если в описании купюры значение номинала соответствует "100", то изменяем
            // фоновый цвет и цвет фона у этой строки
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Denomination == "100")
            {
                e.Row.Background = Brushes.Red;
                e.Row.Foreground = Brushes.White;
            }
        }
    }
}