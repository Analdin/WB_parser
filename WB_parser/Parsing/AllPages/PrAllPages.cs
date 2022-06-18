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
                _path = @"C:\Клиентам\Илья(I)\allUrls.txt";

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
                        write.Flush();
                    }
                }

                //читаем файл со ссылками, парсим href с получившихся ссылок
                using(StreamReader reader = new StreamReader(_path))
                {
                    IDocument subDoc;
                    string? line;

                    while((line = await reader.ReadLineAsync()) != null)
                    {
                        ConsoleColors.DrawColor("Green", $"Читаем строку: {line}");

                        if (!line.Contains("wildberries"))
                        {
                            subDoc = await context.OpenAsync("https://www.wildberries.ru/" + line);
                        }
                        else
                        {
                            subDoc = await context.OpenAsync(line);
                        }

                        IHtmlCollection<IElement> allSubHref = subDoc.QuerySelectorAll("a");

                        foreach(var sub in allSubHref)
                        {
                            var subLink = sub.GetAttribute("href").ToString();

                            ConsoleColors.DrawColor("Green", $"Читаем sub строку: {subLink}");

                            using (StreamWriter write = new StreamWriter(_path, true, System.Text.Encoding.UTF8))
                            {
                                await write.WriteAsync(subLink + "\n");
                                write.Flush();
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
