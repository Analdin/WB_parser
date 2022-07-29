using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;
using WB_parser.Color;
using WB_parser.ExcelJob;
using WB_parser.Variable;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WB_parser.DataBase;

namespace WB_parser.Parsing.AllPages
{
    public class HtmlCodeGet
    {
        string _url; // _primer - приватные переменные в классе
        string _path; // primer - локальная переменная в методах

        public HtmlCodeGet(string url, string path)
        {
            _url = url;
            _path = path;
        }

        /// <summary>
        /// Работа парсера, в конце запись полученных данных в таблицу
        /// </summary>
        public async void ParserRun()
        {
            try
            {
                DbHelper bdhelp = new DbHelper();

                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("log-level=3");
                IWebDriver driver = new ChromeDriver(chromeOptions);

                List<string> allLinks = new List<string>();
                List<string> linksLst = new List<string>();

                // -- Заходим на главную сайта, парсим все ссылки с нее --
                _path = Directory.GetCurrentDirectory() + @"\Urls\allUrls.txt";
                ConsoleColors.DrawColor("Gray", $"_path - {_path}");

                Console.ReadLine();

                string[] lines = File.ReadAllLines(_path);
                if (lines.Length == 0)
                {
                    ConsoleColors.DrawColor("Gray", $"Файл txt пуст, записываем его");
                }
                else
                {
                    ConsoleColors.DrawColor("Gray", $"Файл txt заполнен, пропускаем его");
                    goto excelWrite;
                }

                var parser = new HtmlParser();
                var config = Configuration.Default.WithDefaultLoader();
                string address = "https://www.wildberries.ru/";
                IBrowsingContext context = BrowsingContext.New(config);
                IDocument document = await context.OpenAsync(address);

                IHtmlCollection<IElement> cells = document.QuerySelectorAll("a");

                foreach (var link in cells)
                {
                    var neededLink = link.GetAttribute("href").ToString();
                    ConsoleColors.DrawColor("Cyan", $"Ссылка: {neededLink}");
                    //Thread.Sleep(500);

                    using (StreamWriter write = new StreamWriter(_path, true, System.Text.Encoding.UTF8))
                    {
                        await write.WriteAsync(neededLink + "\n");
                    }
                }

                // -- Конец парсинга главной --

                using (FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fs))
                    {
                        IDocument subDoc;
                        string? line;

                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            ConsoleColors.DrawColor("Green", $"Читаем строку: {line}");

                            if (!line.Contains("wildberries"))
                            {
                                subDoc = await context.OpenAsync("https://www.wildberries.ru/" + line);
                                IHtmlCollection<IElement> cells1 = subDoc.QuerySelectorAll("a");

                                foreach (var link in cells1)
                                {
                                    var subLink = link.GetAttribute("href").ToString();
                                    ConsoleColors.DrawColor("Cyan", $"Пишем sub ссылку: {subLink}");

                                    allLinks.Add(subLink + "\n");
                                }
                            }
                            else if (line.Contains("vsemrabota") | line.Contains("seller") | line.Contains("travel") | line.Contains("t.me") | line.Contains("play.google.com")
                                | line.Contains("apps.apple.com") | line.Contains("appgallery8") | line.Contains("vk.com") | line.Contains("odnoklassniki")
                                | line.Contains("youtube") | line.Contains("wbdevs"))
                            {
                                ConsoleColors.DrawColor("Cyan", $"Пропуск строки: {line}");
                            }
                        }
                    }
                }

                linksLst = allLinks.Distinct().ToList();

                // Записываем все в файл
                using (StreamWriter toFile = new StreamWriter(_path))
                {
                    foreach (var link in linksLst)
                    {
                        toFile.Write(link);
                        ConsoleColors.DrawColor("DarkGray", $"Записываем в файл строку: {link}");
                    }
                }

                allLinks.Clear();

                //читаем файл со ссылками, парсим href с получившихся ссылок с главной
                using (StreamReader file = new StreamReader(_path))
                {
                    IDocument subDoc;
                    string? line;

                    while ((line = await file.ReadLineAsync()) != null)
                    {
                        ConsoleColors.DrawColor("Green", $"Читаем строку: {line}");

                        if (!line.Contains("wildberries"))
                        {
                            subDoc = await context.OpenAsync("https://www.wildberries.ru/" + line);
                            IHtmlCollection<IElement> cells1 = subDoc.QuerySelectorAll("a");

                            foreach (var link in cells1)
                            {
                                var subLink = link.GetAttribute("href").ToString();
                                ConsoleColors.DrawColor("Cyan", $"Пишем sub ссылку: {subLink}");

                                allLinks.Add(subLink + "\n");
                            }
                        }
                        else if (line.Contains("vsemrabota") | line.Contains("seller") | line.Contains("travel") | line.Contains("t.me") | line.Contains("play.google.com")
                            | line.Contains("apps.apple.com") | line.Contains("appgallery8") | line.Contains("vk.com") | line.Contains("odnoklassniki")
                            | line.Contains("youtube") | line.Contains("wbdevs"))
                        {
                            ConsoleColors.DrawColor("Cyan", $"Пропуск строки: {line}");
                        }
                    }
                }

                // Записываем все в txt файл
                using (StreamWriter toFile = new StreamWriter(_path))
                {
                    foreach (var link in allLinks)
                    {
                        toFile.Write(link);
                        ConsoleColors.DrawColor("DarkGray", $"Записываем в файл строку: {link}");
                    }
                }

            excelWrite:

                int colNum;
                int rowNum;

                colNum = 2;
                rowNum = 1;

                using (StreamReader file = new StreamReader(_path))
                {
                    string? line = String.Empty;
                    string lineWithWb = String.Empty;

                    while ((line = await file.ReadLineAsync()) != null)
                    {
                        ConsoleColors.DrawColor("Cyan", $"Получил строку с файла: {line}");

                        if (!line.Contains("wildberries.ru"))
                        {
                            lineWithWb = "https://www.wildberries.ru" + line;

                            var request = new GetRequest(lineWithWb);
                            request.Run();

                            ConsoleColors.DrawColor("DarkGray", $"Получен html код страницы1: {lineWithWb}");

                            driver.Navigate().GoToUrl(lineWithWb);

                            Thread.Sleep(2000);

                            Actions actions = new Actions(driver);
                            actions.SendKeys(Keys.PageDown).Build().Perform();

                            Thread.Sleep(2000);

                            List<IWebElement> elms = driver.FindElements(By.XPath("//span[contains(@class,'goods-card__price-now')]|//p[contains(@class, 'goods-card__price')]/span")).ToList();

                            foreach (IWebElement elm in elms)
                            {
                                rowNum++;
                                VariablesForReport.tovPriceWithDiscount = elm.GetAttribute("innerText");
                                ConsoleColors.DrawColor("DarkGray", $"Получили цену со скидкой1: {VariablesForReport.tovPriceWithDiscount}");

                                JobWithExcel.ExcJob(1, colNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovPriceWithDiscount);
                            }

                            //driver.Close();
                        }
                        else
                        {
                            var request = new GetRequest(line);
                            request.Run();

                            ConsoleColors.DrawColor("DarkGray", $"Получен html код страницы2: {line}");

                            driver.Navigate().GoToUrl(line);

                            Thread.Sleep(5000);

                            Actions actions = new Actions(driver);
                            actions.SendKeys(Keys.PageDown).Build().Perform();

                            Thread.Sleep(5000);

                            // Парсим контейнеры, внутри то что нужно по товарам
                            while (true)
                            {

                                List<IWebElement> parents = driver.FindElements(By.XPath("//ul[contains(@class, 'swiper-wrapper')]")).ToList();

                                Console.WriteLine("Нашли товаров: " + parents.Count);


                                foreach (IWebElement parent in parents)
                                {

                                    string zagolovok = parent.FindElement(By.XPath(".//a[contains(@class, 'bull-item__self-link auto-shy')]")).GetAttribute("innerText");

                                    string priceWithDiscount = parent.FindElement(By.XPath(".//del[contains(@class, 'goods-card__price-last')]|//span[contains(@class, 'price-old-block')]/del")).GetAttribute("innerText");

                                    string tovName = parent.FindElement(By.XPath(".//p[contains(@class, 'goods-card__description')]/span|//span[contains(@class, 'goods-name')]")).GetAttribute("innerText");

                                    string cardNum = parent.FindElement(By.XPath(".//p[contains(@class, 'goods-card__description')]/span|//span[contains(@class, 'goods-name')]")).GetAttribute("innerText");

                                    //здесь сохраняем спарсенную позицию.

                                }

                                IWebElement nextPage = driver.FindElement(By.XPath("//dalee"));

                                if (nextPage.) break;
                                nextPage.Click();
                                instance.ActiveTab.WaitDownloading();
                            }

                            // Парсинг цен со скидкой
                            List<IWebElement> elms = driver.FindElements(By.XPath("//span[contains(@class,'goods-card__price-now')]|//p[contains(@class, 'goods-card__price')]/span|//ins[contains(@class, 'lower-price')]")).ToList();

                            // Парсинг цен без скидки
                            List<IWebElement> elmsWithoutDiscount = driver.FindElements(By.XPath("//del[contains(@class, 'goods-card__price-last')]|//span[contains(@class, 'price-old-block')]/del")).ToList();

                            // Парсинг наименований товара
                            List<IWebElement> tovNames = driver.FindElements(By.XPath("//p[contains(@class, 'goods-card__description')]/span|//span[contains(@class, 'goods-name')]")).ToList();

                            // Парсинг артикулов
                            List<IWebElement> cardNums = driver.FindElements(By.XPath("//li[contains(@class, 'goods-card')]|//div[contains(@class, 'product-card-overflow')]/div/div[contains(@id,'')]")).ToList();

                            IWebElement title = driver.FindElement(By.XPath("//h1[contains(@class, 'catalog-title')]"));
                            string titleTxt = title.GetAttribute("innerText");

                            if (String.IsNullOrWhiteSpace(Variables.category))
                            {
                                ConsoleColors.DrawColor("DarkGray", $"Параметр 'Категория' - не заполнен");
                            }
                            else if (Variables.category.Trim().ToLower() == titleTxt.Trim().ToLower())
                            {
                                ConsoleColors.DrawColor("DarkGray", $"Сбор данных будет только из категории {titleTxt}");
                            }

                            colNum = 1;

                            foreach (IWebElement elm in tovNames)
                            {
                                rowNum++;
                                VariablesForReport.tovName = elm.GetAttribute("innerText");
                                ConsoleColors.DrawColor("DarkGray", $"Получили наименование товара: {VariablesForReport.tovName}");

                                JobWithExcel.ExcJob(1, colNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovName);
                            }

                            colNum = 2;

                            foreach (IWebElement elm in elms)
                            {
                                rowNum++;
                                VariablesForReport.tovPriceWithDiscount = elm.GetAttribute("innerText");
                                ConsoleColors.DrawColor("DarkGray", $"Получили цену со скидкой: {VariablesForReport.tovPriceWithDiscount}");

                                VariablesForReport.tovPriceWithDiscount = VariablesForReport.tovPriceWithDiscount.Replace("₽", "");

                                JobWithExcel.ExcJob(1, colNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovPriceWithDiscount);
                            }

                            colNum = 3;

                            foreach (IWebElement elm in elmsWithoutDiscount)
                            {
                                rowNum++;
                                VariablesForReport.tovPriceWithoutDiscount = elm.GetAttribute("innerText");
                                ConsoleColors.DrawColor("DarkGray", $"Получили цену без скидки: {VariablesForReport.tovPriceWithoutDiscount}");

                                VariablesForReport.tovPriceWithoutDiscount = VariablesForReport.tovPriceWithoutDiscount.Replace("₽", "");

                                JobWithExcel.ExcJob(1, colNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovPriceWithoutDiscount);
                            }

                            colNum = 4;

                            foreach (IWebElement elm in cardNums)
                            {
                                rowNum++;
                                VariablesForReport.cardNum = elm.GetAttribute("data-popup-nm-id");
                                ConsoleColors.DrawColor("DarkGray", $"Получили артикул товара: {VariablesForReport.cardNum}");

                                JobWithExcel.ExcJob(1, colNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.cardNum);
                            }

                            //driver.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}