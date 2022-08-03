using WB_parser.Menu;
using WB_parser.Login;
using WB_parser.Parsing;
using WB_parser.Parsing.AllPages;
using WB_parser.Color;
using WB_parser.Variable;
using WB_parser.TelegramJob;

namespace MainJob
{
    public class Program
    {
        public static bool statusOfLogin { get; set; } = false;
        public static string timeStart { get; set; }
        public static string timeEnd { get; set; }

        static void Main(string[] args)
        {

            string[] menuItems = new string[] { "Логин", "Парсинг", "Категория: выбор", "Подкатегория: выбор", "Цена: выбор", "Уровень скидки", "Период", "Выход" };

            int row = Console.CursorTop;
            int col = Console.CursorLeft;
            int index = 0;

            TelegramSendCard.BotStart();

            Console.WriteLine("Меню");
            Console.WriteLine();

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
                        if (index > 0)
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

                                if (str == "Y")
                                {
                                    ConsoleColors.DrawColor("Cyan", $"Проверяем логин и пароль для входа..");

                                    if (statusOfLogin)
                                    {
                                        ConsoleColors.DrawColor("Cyan", $"Пароль и логин - верные, запускаем парсинг..");
                                        Thread.Sleep(500);

                                        //Работа парсера
                                        ConsoleColors.DrawColor("Cyan", $"Парсинг сайта запущен..");
                                        //Console.Clear();

                                        //Запрос к wildberries
                                        var request = new GetRequest("https://www.wildberries.ru/");
                                        request.Run();

                                        ConsoleColors.DrawColor("Cyan", $"Сбор url запущен..");

                                        var parsePages = new HtmlCodeGet(request.ToString(), @"\Urls\allUrls.txt");
                                        parsePages.ParserRun();

                                        ConsoleColors.DrawColor("Cyan", $"Сбор url закончен.");

                                        Thread.Sleep(500);
                                        //Console.Clear();

                                        goto default;
                                    }
                                    else
                                    {
                                        ConsoleColors.DrawColor("Red", $"Не верные логин и пароль, попробуйте ввести еще раз..");
                                        Thread.Sleep(1500);
                                        //Console.Clear();
                                        goto case 0;
                                    }
                                }
                                else
                                {
                                    ConsoleColors.DrawColor("Cyan", $"Отменяем парсинг..");
                                    goto default;
                                }
                            case 2:
                                ConsoleColors.DrawColor("Cyan", $"Введите категорию товара..");
                                Variables.category = Console.ReadLine();
                                ConsoleColors.DrawColor("DarkGray", $"Записана категория: {Variables.category}");

                                Thread.Sleep(500);

                                goto case 3;
                            case 3:
                                ConsoleColors.DrawColor("Cyan", $"Выберите подкатегорию товара..");
                                Variables.subCategory = Console.ReadLine();
                                ConsoleColors.DrawColor("DarkGray", $"Записана подкатегория: {Variables.subCategory}");

                                goto case 4;
                            case 4:
                                ConsoleColors.DrawColor("Cyan", $"Выберите диапазон цены товара (пример 1500 - 4500)..");
                                Variables.priceChoose = Console.ReadLine();
                                ConsoleColors.DrawColor("DarkGray", $"Выбран диапазон цен: {Variables.priceChoose}");

                                goto case 5;
                            case 5:
                                ConsoleColors.DrawColor("Cyan", $"Выберите период за который нужно собрать скидки. (Пример: 01.06.22 - 12.06.22)");

                                ConsoleColors.DrawColor("Cyan", $"Проверяем логин и пароль для входа..");

                                if (statusOfLogin)
                                {
                                    ConsoleColors.DrawColor("Green", $"Пароль и логин - верные, устанавливаем дату для парсинга..");

                                    ConsoleColors.DrawColor("DarkGray", $"Введите начальную дату.");
                                    timeStart = Console.ReadLine();

                                    if (timeStart.Length > 10)
                                    {
                                        ConsoleColors.DrawColor("Red", $"Введите верный формат начальной даты (пример: 01.06.2022)");
                                    }
                                    else
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Записана начальная дата: {timeStart}");
                                    }

                                    ConsoleColors.DrawColor("DarkGray", $"Введите конечную дату.");
                                    timeEnd = Console.ReadLine();

                                    if (timeEnd.Length > 10)
                                    {
                                        ConsoleColors.DrawColor("Red", $"Введите верный формат конечной даты (пример: 01.06.2022)");
                                    }
                                    else
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Записана начальная дата: {timeEnd}");
                                    }

                                    ConsoleColors.DrawColor("Green", $"Установите размер скидки, если не нужно, нажмите Enter. (Устанавливается без знака '%')");
                                    Variables.discountSet = Console.ReadLine();
                                    if (String.IsNullOrWhiteSpace(Variables.discountSet))
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Установка скидки пропущена: {Variables.discountSet}");
                                        goto case 1;
                                    }
                                    else
                                    {
                                        ConsoleColors.DrawColor("DarkGray", $"Установлена скидка: {Variables.discountSet}");
                                        goto case 1;
                                    }
                                }
                                else
                                {
                                    ConsoleColors.DrawColor("Red", $"Не верные логин и пароль, попробуйте ввести еще раз..");
                                    goto case 0;
                                }

                                break;
                            case 6:
                                ConsoleColors.DrawColor("Cyan", $"Установка периода");
                                return;
                            case 7:
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