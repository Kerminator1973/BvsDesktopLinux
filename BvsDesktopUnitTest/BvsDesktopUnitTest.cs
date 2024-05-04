using BvsDesktopLinux.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BvsDesktopTests
{
    [TestClass]
    public class UnitTestViewModel
    {
        public MainWindowViewModel mainViewModel = new();

        // ������ �� ������������� ������:
        // https://www.c-sharpcorner.com/UploadFile/dacca2/test-initialize-and-test-setup/
        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void TestRestoreCounts()
        {
            var count = mainViewModel.RestoreCounts();

            // ������ ���� ������ ��������, � ������ ������� - "RUB"
            Assert.IsTrue(count == 4);
            Assert.IsTrue(mainViewModel.Banknotes[0].Currency == "RUB");
        }

        [TestMethod]
        public void TestDeleteBanknote()
        {
            var count = mainViewModel.RestoreCounts();

            Assert.IsTrue(count == 4);

            // ������� ������������� ������� ��������� ������� �� ������� ������� ������
            mainViewModel.SelectedBanknote = mainViewModel.Banknotes[0];

            var denomination = mainViewModel.SelectedBanknote.Denomination;

            // �������� ������� �������� ������ DataGrid
            mainViewModel.DeleteBanknote();

            // ���������, ��� ���������� ������� ����������� �� ���� � �� �����
            // �������� ������ ��� ��������� ������ ������
            count = mainViewModel.Banknotes.Count;
            Assert.IsTrue(count == 3);
            Assert.IsTrue(mainViewModel.Banknotes[0].Denomination != denomination);
        }
    }
}
