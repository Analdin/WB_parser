namespace WB_parser.Menu
{
    public class DrawMenu
    {
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
