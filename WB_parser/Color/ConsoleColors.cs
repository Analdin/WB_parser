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
        public static void DrawColor(string name, string message)
        {
            if(name == "")
            {
                Console.ResetColor();
            }
            else if(name == "Red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if(name == "Green")
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if(name == "DarkGray")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else if(name == "Cyan")
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
