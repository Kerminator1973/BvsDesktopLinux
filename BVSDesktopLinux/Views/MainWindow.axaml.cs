using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

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
            DataGridRow row = e.Row;
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Denomination == "100")
            {
                e.Row.Background = Brushes.Red;
            }
        }
    }
}