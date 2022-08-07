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
using WB_parser.TelegramJob;

namespace WB_parser.Parsing.AllPages
{
    public class HtmlCodeGet
    {
        string _url; // _primer - приватные переменные в классе
        string _path; // primer - локальная переменная в методах

        private bool PauseCheck(char input)
        {
            //var input = char.ToLower(Console.ReadKey(true).KeyChar);
            if (input == 'p' || input == 'з')
            {
                ConsoleColors.DrawColor("Cyan", "Программа остановлена для продолжения нажмите Enter");
                Console.ReadLine();
            }
            else if (input == 'm' || input == 'ь')
            {
                ConsoleColors.DrawColor("Green", $"Выход в меню");
                Thread.Sleep(500);
                ConsoleColors.Clear();
                return true;
            }
            return false;
        }

        public HtmlCodeGet(string url, string path)
        {
            _url = url;
            _path = path;
        }

        /// <summary>
        /// Работа парсера, в конце запись полученных данных в таблицу
        /// </summary>
        public async Task ParserRun()
        {

            try
            {
                DbHelper bdhelp = new DbHelper();

                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("log-level=3");
                IWebDriver driver = new ChromeDriver(chromeOptions);

                List<string> allLinks = new List<string>();
                List<string> linksLst = new List<string>();

                bdhelp.OpenConnection();

                int bdRowCount;
                var query00 = $"SELECT COUNT(*) FROM `parser_report`";
                var command00 = new MySqlCommand(query00, bdhelp.Connection);
                var scalar = command00.ExecuteScalar();
                if (scalar == null || !int.TryParse(scalar.ToString(), out bdRowCount))
                    bdRowCount = 0;

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

                while (true)
                {
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

                            int mn = 0;
                            int m = 0;

                            Thread.Sleep(10000);

                            List<IWebElement> headerList = driver.FindElements(By.XPath("//ul[contains(@class, 'breadcrumbs__list')]/li/a/span|//ul[contains(@class, 'breadcrumbs__list')]/li/span")).ToList();
                            string category = "", subcategory = "";
                            if (headerList != null)
                            {
                                if (headerList.Count > 1)
                                    category = headerList[1].Text;
                                if (headerList.Count > 2)
                                    subcategory = headerList[2].Text;
                                if (category == "")
                                    ConsoleColors.DrawColor("Red", $"Заголовок не найден");
                            }
                            else
                                ConsoleColors.DrawColor("Red", $"Заголовки не найдены");

                            char inputKey = '_';

                            int curePage = 0;

                            // Парсим контейнеры, внутри то что нужно по товарам

                            while (true)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    Actions actions = new Actions(driver);
                                    actions.SendKeys(Keys.PageDown).Build().Perform();

                                    Thread.Sleep(2000);
                                }
                                List<IWebElement> allElms = driver.FindElements(By.XPath("//li[contains(@class, 'j-product-item')]|//div[contains(@class, 'j-card-item')]")).ToList();

                                ConsoleColors.DrawColor("DarkGray", $"allElms: {allElms.Count}");
                                mn++;
                                ConsoleColors.DrawColor("DarkGray", $"mn: {mn}");
                                foreach (var elm in allElms)
                                {
                                    if (Console.KeyAvailable)
                                        inputKey = char.ToLower(Console.ReadKey(true).KeyChar);
                                    if (inputKey != '_')
                                    {
                                        if (PauseCheck(inputKey))
                                            return;
                                        inputKey = '_';
                                    }
                                    m++;
                                    ConsoleColors.DrawColor("DarkGray", $"m: {m}");

                                    try
                                    {
                                        VariablesForReport.tovName = elm.FindElement(By.XPath(".//p[contains(@class, 'goods-card__description')]/span|.//span[contains(@class, 'goods-name')]")).Text;
                                        if (VariablesForReport.tovName.LastOrDefault(' ') == '\\')
                                            VariablesForReport.tovName = VariablesForReport.tovName.Substring(0, VariablesForReport.tovName.Length - 1);
                                        ConsoleColors.DrawColor("DarkGray", $"Имя товара: {VariablesForReport.tovName}");
                                    }
                                    catch (NoSuchElementException ex)
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                    }

                                    try
                                    {
                                        VariablesForReport.tovPriceWithDiscount = elm.FindElement(By.XPath(".//span[contains(@class, 'price')]/ins[contains(@class, 'lower-price')]|.//span[contains(@class, 'price')]/span[contains(@class, 'lower-price')]|.//p[contains(@class, 'goods-card__price')]/span")).Text;
                                        VariablesForReport.tovPriceWithDiscount = System.Text.RegularExpressions.Regex.Replace(VariablesForReport.tovPriceWithDiscount, @"[^\d]", "");
                                        ConsoleColors.DrawColor("DarkGray", $"Цена со скидкой: {VariablesForReport.tovPriceWithDiscount}");
                                    }
                                    catch (NoSuchElementException ex)
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Элемент не найден: {ex.Message}");
                                    }
                                    if (Console.KeyAvailable)
                                        inputKey = char.ToLower(Console.ReadKey(true).KeyChar);
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
                                    if (Console.KeyAvailable)
                                        inputKey = char.ToLower(Console.ReadKey(true).KeyChar);
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

                                    try
                                    {
                                        VariablesForReport.discount = elm.FindElement(By.XPath(".//p[contains(@class, 'goods-card__discount')]|.//span[contains(@class, 'product-card__sale')]")).Text.Replace("-", "").Replace("%", "").Trim();
                                    }
                                    catch (NoSuchElementException ex)
                                    {
                                        VariablesForReport.discount = "0";
                                    }

                                    if (bdhelp.Connection.State == System.Data.ConnectionState.Closed)
                                        bdhelp.OpenConnection();

                                    int tovPriceWithoutDiscount, tovPriceWithDiscount;
                                    if (!int.TryParse(VariablesForReport.tovPriceWithoutDiscount, out tovPriceWithoutDiscount))
                                    {
                                        tovPriceWithoutDiscount = -1;
                                    }
                                    if (!int.TryParse(VariablesForReport.tovPriceWithDiscount, out tovPriceWithDiscount))
                                    {
                                        tovPriceWithDiscount = -1;
                                        ConsoleColors.DrawColor("DarkGray", $"Некорректная цена со скидкой");
                                    }
                                    if (Console.KeyAvailable)
                                        inputKey = char.ToLower(Console.ReadKey(true).KeyChar);
                                    if (bdRowCount < 2)
                                    {
                                        var query = $"INSERT INTO `parser_report`(`product_name`, `price_w_discount`, `price_without_discount`, `category`, `subcategory`, `vendor_code`, " +
                                            $"`difference`) VALUES('{VariablesForReport.tovName.Replace('\'', '"').Replace("`", "")}', '{VariablesForReport.tovPriceWithDiscount}', " +
                                            $"'{VariablesForReport.tovPriceWithoutDiscount}', '{category}', '{subcategory}', '{Variables.cardNumId}', " +
                                            $"{(tovPriceWithoutDiscount != -1 && tovPriceWithDiscount != -1 ? tovPriceWithoutDiscount - tovPriceWithDiscount : 0)})";
                                        var command = new MySqlCommand(query, bdhelp.Connection);
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        int discountStart = -1, discountEnd = -1, discountDeclared;
                                        if (Variables.discountSet != "")
                                        {
                                            var discountArray = Variables.discountSet.Trim().Split('-');
                                            if (discountArray.Length != 2 || !int.TryParse(discountArray[0], out discountStart) || !int.TryParse(discountArray[1], out discountEnd))
                                                discountStart = discountEnd = -1;
                                        }

                                        if (!int.TryParse(VariablesForReport.discount, out discountDeclared))
                                            discountDeclared = 0;
                                        if ((Variables.category.ToLower() == "" || (category.ToLower() == Variables.category.ToLower() &&
                                            (Variables.subCategory.ToLower() == "" || subcategory.ToLower() == Variables.subCategory.ToLower())))
                                            && (discountStart == -1 || (discountStart <= discountDeclared && discountDeclared <= discountEnd)))
                                        {
                                            int tovPriceWithDiscount_Old;
                                            var query0 = $"SELECT `price_w_discount` FROM `parser_report` WHERE `vendor_code` = '{Variables.cardNumId}'";
                                            var command0 = new MySqlCommand(query0, bdhelp.Connection);
                                            var scalar1 = command0.ExecuteScalar();
                                            if (scalar1 == null || !int.TryParse(scalar1.ToString(), out tovPriceWithDiscount_Old))
                                                tovPriceWithDiscount_Old = -1;
                                            if (scalar1 != null)
                                                if (tovPriceWithDiscount != -1 && tovPriceWithDiscount_Old != -1)
                                                {
                                                    int difference = tovPriceWithDiscount - tovPriceWithDiscount_Old;
                                                    int differencePercent = difference * 100 / tovPriceWithDiscount_Old;
                                                    var query2 = $@"UPDATE `parser_report` SET `price_lower`='{(difference < 0 ? -difference : 0)}', `price_higher`='
                                                        {(difference > 0 ? difference : 0)}', `percent`={differencePercent} WHERE `vendor_code` = '{Variables.cardNumId}'";
                                                    var command2 = new MySqlCommand(query2, bdhelp.Connection);
                                                    command2.ExecuteNonQuery();

                                                    string productUrl = @$"https://www.wildberries.ru/catalog/{Variables.cardNumId}/detail.aspx";
                                                    if (difference != 0)
                                                        TelegramSendCard.SendMessages(VariablesForReport.tovName, Variables.isPersent ? differencePercent.ToString() + '%' :
                                                            difference.ToString(), Variables.cardNumId, productUrl);
                                                }
                                        }
                                    }
                                    if (Console.KeyAvailable)
                                        inputKey = char.ToLower(Console.ReadKey(true).KeyChar);
                                }


                                try
                                {
                                    // Пагинация
                                    if (++curePage < Variables.paginationMax)
                                    {
                                        IWebElement nextPage = driver.FindElement(By.XPath("//a[contains(@class, 'pagination-next pagination__next')]"));
                                        driver.Navigate().GoToUrl(nextPage.GetAttribute("href"));
                                    }
                                    else
                                        throw new Exception();
                                }
                                catch
                                {
                                    ConsoleColors.DrawColor("DarkGray", $"m >= allElms.Count: {m}");
                                    bdhelp.CloseConnection();
                                    break;
                                }
                            }
                        }
                    }
                    if (bdRowCount == 0)
                    {
                        bdhelp.OpenConnection();
                        query00 = $"SELECT COUNT(*) FROM `parser_report`";
                        command00 = new MySqlCommand(query00, bdhelp.Connection);
                        scalar = command00.ExecuteScalar();
                        if (scalar == null || !int.TryParse(scalar.ToString(), out bdRowCount))
                            bdRowCount = 0;
                        bdhelp.CloseConnection();
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