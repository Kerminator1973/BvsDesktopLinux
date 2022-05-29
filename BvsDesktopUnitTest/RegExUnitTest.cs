using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace BvsDesktopTests
{
    [TestClass]
    public class RegExUnitTest
    {
        private readonly string htmlExample = new(@"
            <tr valign=""middle"">
              < td align = ""center"" width = ""60%"" colspan = ""2"" >
                < form method = ""GET"" action = ""http://192.168.1.254/ser_num/00001321.zip"" >
                  < input type = ""submit"" class=""CSS_MONO_FONT"" style=""height:50px; width:100%"" value=""N   1:      | 00001321 |        8 Banknotes"">
                </form>
              </td>
              <td align = ""left"" class=""CSS_MONO_FONT"" colspan=""2"">
                <a href = ""http://192.168.1.254/ser_num/00001321.zip"" >/ ser_num / 00001321.zip </ a >
                < br > 401 bytes
              </td>
            </tr>
            <tr valign = ""middle"" >
              < td align=""center"" width=""60%"" colspan=""2"">
                <form method = ""GET"" action=""http://192.168.1.254/ser_num/00001322.zip"">
                  <input type = ""submit"" class=""CSS_MONO_FONT"" style=""height:50px; width:100%"" value=""N   2:      | 00001322 |        5 Banknotes"">
                </form>
              </td>
              <td align = ""left"" class=""CSS_MONO_FONT"" colspan=""2"">
                <a href = ""http://192.168.1.254/ser_num/00001322.zip"" >/ ser_num / 00001322.zip </ a >
                < br > 394 bytes
              </td>
            </tr>
        ");

        private readonly string htmlUinHeader = new(@"
            <p style=""text-align: center; margin: 0px; padding: 10px; font: bold 20px serif"">UIN: D820-001-00000775</p>
        ");

        private void matchUIN(Regex reUIN)
        {
            Match m = reUIN.Match(htmlUinHeader);
            Assert.IsTrue(m.Success);
            Assert.IsTrue(m.Value.Length > 9);

            var uin = m.Value.Substring(5, m.Value.Length - 9).Trim();
            Assert.IsTrue(uin == "D820-001-00000775");
        }

        [TestMethod]
        public void TestRegExSimple()
        {
            var reUIN = new Regex(@">UIN:.*?<\/p>");
            for(int i = 0; i < 100000; i++)
            {
                matchUIN(reUIN);
            }
        }

        [TestMethod]
        public void TestRegExSimpleCompiled()
        {
            var reUIN = new Regex(@">UIN:.*?<\/p>", RegexOptions.Compiled);
            for (int i = 0; i < 100000; i++)
            {
                matchUIN(reUIN);
            }
        }
    }
}

