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
            int count = mainViewModel.Banknotes.Count;

            // Должно быть четыре элемента, а валюта каждого - "CNY" (китайские юани)
            Assert.IsTrue(count == 4);
            Assert.IsTrue(mainViewModel.Banknotes[0].Currency == "CNY");
        }
    }
}
