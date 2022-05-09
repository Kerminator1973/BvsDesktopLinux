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

            // ������ ���� ������ ��������, � ������ ������� - "CNY" (��������� ����)
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

            // ������� ������������� ������� ��������� ������� �� ������� ������� ������
            mainViewModel.SelectedBanknote = mainViewModel.Banknotes[0];

            var denomination = mainViewModel.Banknotes[0].Denomination;

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
