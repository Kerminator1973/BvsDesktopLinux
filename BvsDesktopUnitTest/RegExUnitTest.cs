using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BvsDesktopTests
{
    [TestClass]
    public class RegExUnitTest
    {
        // Пример части HTML-документа генерируемым прибором D820F.
        // https://github.com/Kerminator1973/BVSDesktopSupport/blob/main/Examples/DORS-8xx.html
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
                <a href = 'http://192.168.1.254/ser_num/00001322.zip' >/ ser_num / 00001322.zip </ a >
                < br > 394 bytes
              </td>
            </tr>
        ");

        // Шаблон для извлечения ссылок на пересчёты купюр

        // Расшифровка старого варианта:
        //private readonly string reHrefPattern = new(@"(?inx)  // Flags: insensitive, no capture, extended
        //                <a \s [^>]*                           // "<a" потом пробел и не ">": т.е. начало тэга <a>
        //                    href \s* = \s*                    // должен быть атрибут "href"
        //                        (?<q> ['""] )                 // Дальше может быть символ кавычка, либо двойная кавычка;
        //                                                          это была Capture Group с именем "q" (quote)
        //                            (?<url> [^""]+ )          // Это Capture Group, которая завершается двойной кавычкой
        //                        \k<q>
        //                [^>]* >");                            // Строка должна завершаться завершением тэга

        private readonly string reHrefPattern = new(@"(?inx)
                        <a\s[^>]*
                            href\s*=\s*
                                (?<q>['""])
                                    (?<url>[^'""]+)
                                \k<q>
                        [^>]*>");

        // Шаблон для извлечения идентификатора устройства
        private readonly string htmlUinHeader = new(@"
            <p style=""text-align: center; margin: 0px; padding: 10px; font: bold 20px serif"">UIN: D820-001-00000775</p>
        ");

        private readonly string reUinPattern = new(@">UIN:.*?<\/p>");

        // Тест извлечения УИН-а
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
            var reUIN = new Regex(reUinPattern);
            for (int i = 0; i < 100000; i++)
            {
                matchUIN(reUIN);
            }
        }

        [TestMethod]
        public void TestRegExSimpleCompiled()
        {
            var reUIN = new Regex(reUinPattern, RegexOptions.Compiled);
            for (int i = 0; i < 100000; i++)
            {
                matchUIN(reUIN);
            }
        }

        [TestMethod]
        public void TestRegExComplexCompiled()
        {
            var reTemplate = new Regex(reHrefPattern, RegexOptions.Compiled);

            List<string> urls = new();
            foreach (Match match in reTemplate.Matches(htmlExample))
            {
                urls.Add(match.Groups["url"].ToString());
            }

            Assert.IsTrue(urls.Count == 2);
            Assert.IsTrue(urls[0] == "http://192.168.1.254/ser_num/00001321.zip");
            Assert.IsTrue(urls[1] == "http://192.168.1.254/ser_num/00001322.zip");
        }
    }
}

