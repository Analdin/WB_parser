using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WB_parser.Color
{
    public class ConsoleColors
    {
        public static void DrawColor(string name, string message)
        {
            if(name == null)
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
            else if(name == "Red")
            {
                Console.ForegroundColor = ConsoleColor.Red;
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
