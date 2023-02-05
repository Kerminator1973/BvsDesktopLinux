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
            // Если в описании купюры значение номинала соответствует "100", то изменяем
            // фоновый цвет и цвет фона у этой строки.
            //
            // Фундаментально подход не правильный, т.к. если потом строки в таблице будут удалены,
            // а вместо них будут добавлены другие строки, изменённое свойство сохраниться.
            // TODO: скорректировать ошибку в использовании подхода
            var dataObject = e.Row.DataContext as Models.Banknote;
            if (dataObject != null && dataObject.Status == "Rejected")
            {
                e.Row.Background = Brushes.Red;
                e.Row.Foreground = Brushes.White;
            }
        }
    }
}