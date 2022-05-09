using BvsDesktopLinux.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BvsDesktopTests
{
    [TestClass]
    public class UnitTestViewModel
    {
        [TestMethod]
        public void TestRestoreCounts()
        {
            MainWindowViewModel mainViewModel = new();
            mainViewModel.RestoreCounts("CNY");
            var count = mainViewModel.Banknotes.Count;

            // Должно быть четыре элемента, а валюта каждого - "CNY" (китайские юани)
            Assert.IsTrue(count == 4);
            Assert.IsTrue(mainViewModel.Banknotes[0].Currency == "CNY");
        }

        [TestMethod]
        public void TestDeleteBanknote()
        {
            MainWindowViewModel mainViewModel = new();
            mainViewModel.RestoreCounts("CNY");
            var count = mainViewModel.Banknotes.Count;
            Assert.IsTrue(count == 4);

            // Вручную устанавливаем текущий выбранный элемент на нулевой элемент списка
            mainViewModel.SelectedBanknote = mainViewModel.Banknotes[0];

            var denomination = mainViewModel.Banknotes[0].Denomination;

            // Вызываем команду удаление строки DataGrid
            mainViewModel.DeleteBanknote();

            // Проверяем, что количество записей уменьшилось на одну и на месте
            // удалённой записи уже находится другая запись
            count = mainViewModel.Banknotes.Count;
            Assert.IsTrue(count == 3);
            Assert.IsTrue(mainViewModel.Banknotes[0].Denomination != denomination);
        }
    }
}
