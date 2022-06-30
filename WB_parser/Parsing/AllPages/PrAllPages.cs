using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using WB_parser.Color;

namespace WB_parser.Parsing.AllPages
{
    public class HtmlCodeGet
    {
        string _url;
        string _path;

        public HtmlCodeGet(string url, string path)
        {
            _url = url;
            _path = path;
        }

        public async void ParserRun()
        {
            try
            {
                // -- Заходим на главную сайта, парсим все ссылки с нее --
                _path = @"C:\Клиентам\Константин_wb\allUrls.txt";
                string path2 = @"C:\Клиентам\Константин_wb\allUrls1.txt";

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
                        write.Close();
                    }
                }

                // -- Конец парсинга главной --

                //читаем файл со ссылками, парсим href с получившихся ссылок с главной
                using (StreamReader reader = new StreamReader(path2))
                {
                    IDocument subDoc;
                    string? line;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        ConsoleColors.DrawColor("Green", $"Читаем строку: {line}");

                        if (!line.Contains("wildberries"))
                        {
                            subDoc = await context.OpenAsync("https://www.wildberries.ru/" + line);
                            IHtmlCollection<IElement> cells1 = document.QuerySelectorAll("a");

                            foreach(var link in cells1)
                            {
                                var subLink = link.GetAttribute("href").ToString();
                                ConsoleColors.DrawColor("Cyan", $"Пишем sub ссылку: {subLink}");

                                using (StreamWriter write = new StreamWriter(_path, true, System.Text.Encoding.UTF8))
                                {
                                    await write.WriteAsync(subLink + "\n");
                                    write.Close();
                                }
                            }
                        }
                        else if (line.Contains("vsemrabota") | line.Contains("seller") | line.Contains("travel") | line.Contains("t.me") | line.Contains("play.google.com")
                            | line.Contains("apps.apple.com") | line.Contains("appgallery8") | line.Contains("vk.com") | line.Contains("odnoklassniki")
                            | line.Contains("youtube") | line.Contains("wbdevs"))
                        {
                            ConsoleColors.DrawColor("Cyan", $"Пропуск строки: {line}");
                        }
                        else
                        {
                            subDoc = await context.OpenAsync(line);
                            IHtmlCollection<IElement> cells2 = document.QuerySelectorAll("a");

                            foreach (var link in cells2)
                            {
                                var subLink = link.GetAttribute("href").ToString();
                                ConsoleColors.DrawColor("Cyan", $"Пишем sub ссылку: {subLink}");

                                using (StreamWriter write = new StreamWriter(path2, true, System.Text.Encoding.UTF8))
                                {
                                    await write.WriteAsync(subLink + "\n");
                                    write.Close();
                                }
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
