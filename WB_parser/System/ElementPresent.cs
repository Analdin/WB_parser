using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WB_parser.System
{
    public class ElementPresent
    {
        /// <summary>
        /// Проверяем - есть ли html элемент на странице
        /// </summary>
        /// <param name="by"> Что проверяем</param>
        /// <returns> Возвращаем состояние элемента - есть или нет</returns>
        public static bool TryFindElement(By by, out IWebElement element)
        {
            element = null;
            IWebDriver driver = new ChromeDriver();

            try
            {
                element = driver.FindElement(by);
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ожидание загрузки html элемента
        /// </summary>
        /// <param name="element"> Элемент который нужно подождать</param>
        /// <returns> Состояние элемента</returns>
        public static bool IsElementVisible(IWebElement element)
        {
            return element.Displayed && element.Enabled;
        }

        /// <summary>
        /// Ожидание загрузки чего-либо, html страницы например
        /// </summary>
        /// <param name="MilSec"> Сколько млсек ждать</param>
        public static void WaitDownloading(int MilSec)
        {
            Thread.Sleep(MilSec);
        }
    }
}
