namespace WB_parser.Menu
{
    public class DrawMenu
    {
        /// <summary>
        /// Отрисовка меню в консоли
        /// </summary>
        /// <param name="items"> Элементы</param>
        /// <param name="row"> Строка</param>
        /// <param name="col"> Колонка</param>
        /// <param name="index"> Номер</param>
        public static void DrawMainMenu(string[] items, int row, int col, int index)
        {
            Console.SetCursorPosition(col, row);
            for(int i = 0; i < items.Length; i++)
            {
                if(i == index)
                {
                    Console.BackgroundColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine(items[i]);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
}
