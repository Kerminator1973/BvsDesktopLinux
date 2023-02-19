using Avalonia.Controls;

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
            // ���� � �������� ������ �������� ������� ������������� "Rejected", �� ��������
            // ����� ����������� ������
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Status == "Rejected")
            {
                e.Row.Classes.Add("rejectedStatus");
            }
            else
            {
                e.Row.Classes.Remove("rejectedStatus");
            }
        }
    }
}