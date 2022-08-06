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
            string menuIsNumberStr = "Отображение: значением", menuIsPercentStr = "Отображение: процентом";
            string[] menuItems = new string[] { "Логин", "Парсинг", "Категория: выбор", "Подкатегория: выбор", "Цена: выбор", "Уровень скидки", "Период", "Количество страниц", menuIsNumberStr, "Выход" };

            int index = 0;

            TelegramSendCard.BotStart();

            Console.WriteLine("Меню");
            Console.WriteLine();

            while (true)
            {
                DrawMenu.DrawMainMenu(menuItems, 0, 0, index);
                ConsoleColors.SaveCursorPosition(true);
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
                        ConsoleColors.SetActualCursorPosition();
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

                                continue;
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
                                        parsePages.ParserRun().Wait();

                                        ConsoleColors.DrawColor("Cyan", $"Сбор url закончен.");
                                        ConsoleColors.DrawColor("Cyan", $"Парсинг сайта завершён..");

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
                                    break;
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
                                ConsoleColors.DrawColor("Cyan", $"Установите диапазон размера скидки, устанавливается без знака '%' (пример 20-30)");
                                Variables.discountSet = Console.ReadLine();
                                ConsoleColors.DrawColor("DarkGray", $"Установлен диапазон скидки: {Variables.discountSet}");
                                break;
                            case 6:
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
                                }
                                else
                                {
                                    ConsoleColors.DrawColor("Red", $"Не верные логин и пароль, попробуйте ввести еще раз..");
                                    goto case 0;
                                }

                                break;
                            case 7:
                                ConsoleColors.DrawColor("Cyan", $"Введите максимальное количество страниц на странице..");
                                int paginationMax;
                                if (int.TryParse(Console.ReadLine(), out paginationMax))
                                {
                                    Variables.paginationMax = paginationMax;
                                    ConsoleColors.DrawColor("DarkGray", $"Записано максимальное количество страниц: {paginationMax}");
                                }
                                else
                                    ConsoleColors.DrawColor("Red", $"Число введено неверно.");
                                break;
                            case 8:
                                Variables.isPersent = !Variables.isPersent;
                                menuItems[index] = Variables.isPersent ? menuIsPercentStr : menuIsNumberStr;
                                break;
                            case 9:
                                ConsoleColors.DrawColor("Cyan", $"Выбран выход из приложения");
                                return;
                            default:
                                //ConsoleColors.DrawColor("Cyan", $"Выбран пункт {menuItems[index]}");
                                //break;
                                ConsoleColors.DrawColor("Cyan", $"Для выхода в меню нажмите Enter..");
                                Console.ReadLine();
                                ConsoleColors.Clear();
                                continue;
                        }
                        ConsoleColors.SaveCursorPosition();
                        break;
                }
            }
        }
    }
}