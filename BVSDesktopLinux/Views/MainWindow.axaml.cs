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
            // ���� � �������� ������ �������� �������� ������������� "100", �� ��������
            // ������� ���� � ���� ���� � ���� ������
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Denomination == "100")
            {
                e.Row.Background = Brushes.Red;
                e.Row.Foreground = Brushes.White;
            }
        }
    }
}