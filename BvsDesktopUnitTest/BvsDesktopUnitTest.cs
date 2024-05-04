using BvsDesktopLinux.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BvsDesktopTests
{
    [TestClass]
    public class UnitTestViewModel
    {
        public MainWindowViewModel mainViewModel = new();

        // Статья об инициализации тестов:
        // https://www.c-sharpcorner.com/UploadFile/dacca2/test-initialize-and-test-setup/
        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void TestRestoreCounts()
        {
            var count = mainViewModel.RestoreCounts();

            // Должно быть четыре элемента, а валюта каждого - "RUB"
            Assert.IsTrue(count == 4);
            Assert.IsTrue(mainViewModel.Banknotes[0].Currency == "RUB");
        }

        [TestMethod]
        public void TestDeleteBanknote()
        {
            var count = mainViewModel.RestoreCounts();

            Assert.IsTrue(count == 4);

            // Вручную устанавливаем текущий выбранный элемент на нулевой элемент списка
            mainViewModel.SelectedBanknote = mainViewModel.Banknotes[0];

            var denomination = mainViewModel.SelectedBanknote.Denomination;

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
