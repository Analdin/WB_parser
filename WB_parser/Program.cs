using WB_parser.Menu;
using WB_parser.Login;
using WB_parser.Parsing;
using System.Text.RegularExpressions;
using WB_parser.Parsing.AllPages;
using AngleSharp.Html.Parser;
using WB_parser.Color;

namespace MainJob
{
    public class Program
    {
        public static bool statusOfLogin { get; set; } = false;
        public static DateTime timeStart { get; set; }
        public static DateTime timeEnd { get; set;}

        static void Main(string[] args)
        {
            string[] menuItems = new string[] { "Логин", "Парсинг", "Период", "Выход" };

            Console.WriteLine("Меню");
            Console.WriteLine();

            int row = Console.CursorTop;
            int col = Console.CursorLeft;
            int index = 0;

            while (true)
            {
                DrawMenu.DrawMainMenu(menuItems, row, col, index);
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.DownArrow:
                        if (index < menuItems.Length - 1)
                            index++;
                        break;
                    case ConsoleKey.UpArrow:
                        if(index > 0)
                            index--;
                        break;
                    case ConsoleKey.Enter:
                        switch (index)
                        {
                            case 0:
                                ConsoleColors.DrawColor("Cyan", $"Введите логин и пароль для входа в приложение");

                                new LogToUpp.Firewell(
                                    new LogToUpp.Attempts(
                                        new LogToUpp.VerboseDiff(
                                            new LogToUpp.Diff(
                                                new LogToUpp.Password(),
                                                new LogToUpp.Input()
                                                )
                                            ), 3
                                        )).Say();

                                Console.ReadLine();

                                statusOfLogin = true;

                                break;
                            case 1:
                                Console.Write("Запустить парсинг ? (Y/N)");
                                string str = Console.ReadLine();

                                if(str == "Y")
                                {
                                    ConsoleColors.DrawColor("Cyan", $"Проверяем логин и пароль для входа..");

                                    if (statusOfLogin)
                                    {
                                        ConsoleColors.DrawColor("Cyan", $"Пароль и логин - верные, запускаем парсинг..");
                                        Thread.Sleep(500);
                                        //Console.Clear();
                                        //Работа парсера
                                        ConsoleColors.DrawColor("Cyan", $"Парсинг сайта запущен..");

                                        //Запрос к wildberries
                                        var request = new GetRequest("https://www.wildberries.ru/");                                     
                                        request.Run();

                                        ConsoleColors.DrawColor("Cyan", $"Сбор url запущен..");

                                        var parsePages = new HtmlCodeGet(request.ToString(), @"\Urls\allUrls.txt");
                                        parsePages.ParserRun();

                                        ConsoleColors.DrawColor("Cyan", $"Сбор url закончен.");

                                        goto default;
                                    }
                                    else
                                    {
                                        ConsoleColors.DrawColor("Red", $"Не верные логин и пароль, попробуйте ввести еще раз..");
                                        Thread.Sleep(1500);
                                        Console.Clear();
                                        goto case 0;
                                    }
                                }
                                else
                                {
                                    ConsoleColors.DrawColor("Cyan", $"Отменяем парсинг..");
                                    goto default;
                                }

                                break;
                            case 2:
                                ConsoleColors.DrawColor("Cyan", $"Выберите период за который нужно собрать скидки. (Пример: 01.06.22 - 12.06.22)");

                                ConsoleColors.DrawColor("Cyan", $"Проверяем логин и пароль для входа..");

                                if (statusOfLogin)
                                {
                                    ConsoleColors.DrawColor("Green", $"Пароль и логин - верные, устанавливаем дату для парсинга..");
                                }
                                else
                                {
                                    ConsoleColors.DrawColor("Red", $"Не верные логин и пароль, попробуйте ввести еще раз..");
                                    goto case 0;
                                }

                                break;
                            case 3:
                                ConsoleColors.DrawColor("Cyan", $"Выбран выход из приложения");
                                return;
                            default:
                                ConsoleColors.DrawColor("Cyan", $"Выбран пункт {menuItems[index]}");
                                break;
                        }
                        break;
                }
            }
        }
    }
}