using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace WB_parser.System
{
    public class ElementPresent
    {
        private bool IsElementPresent(By by)
        {
            IWebDriver driver = new ChromeDriver();

            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
