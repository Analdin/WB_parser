using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using WB_parser.Color;
using WB_parser.ExcelJob;
using WB_parser.Variable;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using WB_parser.DataBase;
using MySql.Data.MySqlClient;
using WB_parser.SystemElms;

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

                //Console.ReadLine();

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

                    using (StreamWriter write = new StreamWriter(_path))
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

                        var request = new GetRequest(line);
                        request.Run();

                        int CountInFile = File.ReadAllLines(_path).Length;

                        //ConsoleColors.DrawColor("DarkGray", $"Получен html код страницы2: {line}");

                        driver.Navigate().GoToUrl(line);

                        Thread.Sleep(5000);

                        for (int i = 0; i < 3; i++)
                        {
                            Actions actions = new Actions(driver);
                            actions.SendKeys(Keys.PageDown).Build().Perform();

                            Thread.Sleep(2000);
                        }

                        int mn = 0;
                        int m = 0;

                        // Парсим контейнеры, внутри то что нужно по товарам

                        bdhelp.OpenConnection();

                        while (true)
                        {
                            List<IWebElement> allElms = driver.FindElements(By.XPath("//li[contains(@class, 'j-product-item')]|//div[contains(@class, 'product-card__wrapper')]")).ToList();

                            ConsoleColors.DrawColor("DarkGray", $"allElms: {allElms.Count}");
                            mn++;
                            ConsoleColors.DrawColor("DarkGray", $"mn: {mn}");

                            foreach (var elm in allElms)
                            {
                                m++;
                                ConsoleColors.DrawColor("DarkGray", $"m: {m}");

                                try
                                {
                                    VariablesForReport.tovName = elm.FindElement(By.XPath(".//p[contains(@class, 'goods-card__description')]/span|.//span[contains(@class, 'goods-name')]")).Text;
                                    ConsoleColors.DrawColor("DarkGray", $"Имя товара: {VariablesForReport.tovName}");
                                }
                                catch (NoSuchElementException ex)
                                {
                                    ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                }

                                try
                                {
                                    VariablesForReport.tovPriceWithDiscount = elm.FindElement(By.XPath(".//span[contains(@class, 'price')]/ins[contains(@class, 'lower-price')]|.//p[contains(@class, 'goods-card__price')]/span")).Text;
                                    VariablesForReport.tovPriceWithDiscount = System.Text.RegularExpressions.Regex.Replace(VariablesForReport.tovPriceWithDiscount, @"[^\d]", "");
                                    ConsoleColors.DrawColor("DarkGray", $"Цена со скидкой: {VariablesForReport.tovPriceWithDiscount}");
                                }
                                catch (NoSuchElementException ex)
                                {
                                    ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                }

                                try
                                {
                                    VariablesForReport.tovPriceWithoutDiscount = elm.FindElement(By.XPath(".//div[contains(@class, 'goods-card__info')]/p/span//following-sibling::del|.//del[contains(@class, 'goods-card__price-last')]|.//span[contains(@class, 'price-old-block')]/del")).Text;
                                    VariablesForReport.tovPriceWithoutDiscount = System.Text.RegularExpressions.Regex.Replace(VariablesForReport.tovPriceWithoutDiscount, @"[^\d]", "");
                                    //ConsoleColors.DrawColor("DarkGray", $"Цена без скидки: {VariablesForReport.tovPriceWithoutDiscount}");
                                }
                                catch (NoSuchElementException ex)
                                {
                                    //ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                    VariablesForReport.tovPriceWithoutDiscount = "";
                                }
                                ConsoleColors.DrawColor("DarkGray", $"Цена без скидки: {VariablesForReport.tovPriceWithoutDiscount}");

                                try
                                {
                                    //string pattern = @"(?<=catalog/).*(?=/detail\.aspx)";
                                    //string pattern = @"(?<=00/).*(?=-1\.jpg)";
                                    //var curCardWE = elm.FindElement(By.XPath(".//div[contains(@class, 'goods-card__container')]/div/img"));
                                    //(new Actions(driver)).MoveToElement(curCardWE).Perform();
                                    //VariablesForReport.cardNum = curCardWE.GetAttribute("src");
                                    //ConsoleColors.DrawColor("DarkGray", $"src: {VariablesForReport.cardNum}");
                                    //div[contains(@class, 'goods-card__container')]/div/img
                                    //li[contains(@class, 'recently-watched__item')]/a|.//a[contains(@class, 'product-card__main')]|.//li[contains(@class, 'j-product-item')]/a
                                    //Variables.cardNumId = System.Text.RegularExpressions.Regex.Match(VariablesForReport.cardNum, pattern).ToString();
                                    Variables.cardNumId = elm.GetAttribute("data-popup-nm-id");
                                    ConsoleColors.DrawColor("DarkGray", $"Id товара: {Variables.cardNumId}");
                                }
                                catch (NoSuchElementException ex)
                                {
                                    ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                }

                                var query = $"INSERT INTO `parser_report`(`product_name`) VALUES('{VariablesForReport.tovName.Replace('\'', '"')}')";
                                var command = new MySqlCommand(query, bdhelp.Connection);
                                command.ExecuteNonQuery();

                                var query1 = $@"UPDATE `parser_report` SET `price_w_discount`='{VariablesForReport.tovPriceWithDiscount}' WHERE `Num` = {m}";
                                var command1 = new MySqlCommand(query1, bdhelp.Connection);
                                command1.ExecuteNonQuery();

                                var query2 = $@"UPDATE `parser_report` SET `price_without_discount`='{VariablesForReport.tovPriceWithoutDiscount}' WHERE `Num` = {m}";
                                var command2 = new MySqlCommand(query2, bdhelp.Connection);
                                command2.ExecuteNonQuery();

                                var query3 = $@"UPDATE `parser_report` SET `vendor_code`='{Variables.cardNumId}' WHERE `Num` = {m}";
                                var command3 = new MySqlCommand(query3, bdhelp.Connection);
                                command3.ExecuteNonQuery();
                            }

                            if (m >= allElms.Count)
                            {
                                ConsoleColors.DrawColor("DarkGray", $"m >= allElms.Count: {m}");
                                bdhelp.CloseConnection();
                                break;
                            }
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