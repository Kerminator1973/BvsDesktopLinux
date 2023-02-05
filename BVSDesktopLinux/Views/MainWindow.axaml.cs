using Avalonia.Controls;
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
            // ���� � �������� ������ �������� �������� ������������� "100", �� ��������
            // ������� ���� � ���� ���� � ���� ������.
            //
            // �������������� ������ �� ����������, �.�. ���� ����� ������ � ������� ����� �������,
            // � ������ ��� ����� ��������� ������ ������, ��������� �������� �����������.
            // TODO: ��������������� ������ � ������������� �������
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Status == "Rejected")
            {
                e.Row.Background = Brushes.Red;
                e.Row.Foreground = Brushes.White;
            }
        }
    }
}