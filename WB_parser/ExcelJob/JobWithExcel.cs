using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using WB_parser.Color;
using WB_parser.Variable;

namespace WB_parser.ExcelJob
{
    public class JobWithExcel
    {
        public static void ExcJob(int sheetNum, string sheetName, string docToOpen, string path, string cellData)
        {
            FileInfo excTable = new FileInfo(path + @"\" + DateTime.Now + ".xlsx");

            Variables.rowData = cellData;
            if(String.IsNullOrWhiteSpace(cellData))
            {
                ConsoleColors.DrawColor("Red", $"Что-то должно быть записано в cellData... Сейчас она пуста.");
            }

            using (ExcelPackage exclPack = new ExcelPackage(excTable))
            {
                // Количество строк
                int sheetsCount = exclPack.Workbook.Worksheets.Count;
                ConsoleColors.DrawColor("DarkGray", $"Количество листов в таблице с отчетом - {sheetsCount}");

                // Берем лист по номеру
                ExcelWorksheet loneSheet = exclPack.Workbook.Worksheets[sheetNum];

                // Количество колонок
                int colCount = loneSheet.Dimension.End.Row;
                ConsoleColors.DrawColor("DarkGray", $"Количество колонок в таблице с отчетом - {colCount}");
                
                // Цикл по 1 колонке
                for(int i = 0; i < colCount; i++)
                {
                    ConsoleColors.DrawColor("DarkGray", $"Работаем с колонкой - {i}");

                    for(int m = 0; m < sheetsCount; m++)
                    {
                        // Цикл по строкам
                        Variables.rowData = loneSheet.Cells[m + 2, i].Value == null ? "" : loneSheet.Cells[m + 2, i].Value.ToString();
                    }
                }

            }
        }
    }
}
