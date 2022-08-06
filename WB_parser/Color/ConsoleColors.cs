using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WB_parser.Color
{
    public class ConsoleColors
    {
        /// <summary>
        /// Простой вызов цветов. 
        /// </summary>
        /// <param name="name">Имя цвета - "Red","Green" и т.д.</param>
        /// <param name="message">Текст сообщения</param>
        /// 

        private static int _cursorRow = -1;
        private static int _cursorColumn = -1;

        public static int CursorRow { get => _cursorRow; }
        public static int CursorColumn { get => _cursorColumn; }

        public static void DrawColor(string name, string message)
        {
            //ConsoleColors.SetActualCursorPosition();
            if (name == "")
            {
                Console.ResetColor();
            }
            else if (name == "Red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if (name == "Green")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if (name == "DarkGray")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if (name == "Cyan")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static void Clear()
        {
            Console.Clear();
            _cursorRow = -1;
            _cursorColumn = -1;
        }

        public static void SaveCursorPosition(bool ifEmpty = false)
        {
            if (!ifEmpty || _cursorRow == -1)
            {
                _cursorRow = Console.CursorTop;
                _cursorColumn = Console.CursorLeft;
            }
        }

        public static void SetActualCursorPosition()
        {
            if (ConsoleColors.CursorRow != -1)
            {
                if (Console.CursorTop != ConsoleColors.CursorRow)
                    Console.CursorTop = ConsoleColors.CursorRow;
                if (Console.CursorLeft != ConsoleColors.CursorColumn)
                    Console.CursorLeft = ConsoleColors.CursorColumn;
            }
        }
    }
}
