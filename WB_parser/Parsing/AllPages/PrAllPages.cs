using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System.Text.RegularExpressions;
using WB_parser.Color;
using WB_parser.ExcelJob;
using WB_parser.Parsing;
using WB_parser.Variable;

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
                List<string> allLinks = new List<string>();
                List<string> linksLst = new List<string>();

                // -- Заходим на главную сайта, парсим все ссылки с нее --
                _path = Directory.GetCurrentDirectory() + @"\Urls\allUrls.txt";

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

                using(FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
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

                // Работаем с регулярками
                // (?<="lower-price">).*?(?=</ins>) - берет цену без скидки


                int colNum;
                int rowNum;

                // Парсим по всем страницам данные по-порядку
                // 1) Наименование товара



                // 2) Цена со скидкой
                string pattern = @"(?<=""lower - price"">).*?(?=</ins>)";

                colNum = 1;
                rowNum = 2;

                using(StreamReader file = new StreamReader(_path))
                {
                    string? line = String.Empty;
                    string lineWithWb = String.Empty;

                    while ((line = await file.ReadLineAsync()) != null)
                    {
                        rowNum++;

                        if (!line.Contains("wildberries.ru"))
                        {
                            lineWithWb = "https://www.wildberries.ru/" + line;

                            GetCode.GetHtmlCode(lineWithWb);
                            ConsoleColors.DrawColor("DarkGray", $"Получен html код страницы: {lineWithWb}");

                            VariablesForReport.tovPriceWithDiscount = Regex.Match(lineWithWb, pattern).ToString();

                            ConsoleColors.DrawColor("DarkGray", $"Получили цену со скидкой: {VariablesForReport.tovPriceWithDiscount}");

                            JobWithExcel.ExcJob(1, 1, rowNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovPriceWithDiscount);
                        }
                        else
                        {
                            GetCode.GetHtmlCode(line);
                            ConsoleColors.DrawColor("DarkGray", $"Получен html код страницы: {line}");

                            VariablesForReport.tovPriceWithDiscount = Regex.Match(line, pattern).ToString();

                            ConsoleColors.DrawColor("DarkGray", $"Получили цену со скидкой: {VariablesForReport.tovPriceWithDiscount}");

                            JobWithExcel.ExcJob(1, 1, rowNum, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovPriceWithDiscount);
                        }
                    }
                }

                // 3) Цена без скидки

                // Вызываем метод для работы с Excel и пишем в него полученные со страниц данные (отдельный метод)
                JobWithExcel.ExcJob(1, 0, 0, Directory.GetCurrentDirectory() + @"\Urls\Discount_Report.xlsx", VariablesForReport.tovName);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
