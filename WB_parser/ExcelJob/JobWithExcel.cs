using OfficeOpenXml;
using WB_parser.Color;
using WB_parser.Variable;

namespace WB_parser.ExcelJob
{
    public class JobWithExcel
    {
        /// <summary>
        /// Метод для работа с Excel таблицей
        /// </summary>
        /// <param name="sheetNum">Номер страницы</param>
        /// <param name="columnNum">Номер колонки с которой нужно начать работу. Обычно = 0</param>
        /// <param name="rowNum">Строка с которой нужно начать работу. Обычно = 0</param>
        /// <param name="path">Путь к документу</param>
        /// <param name="cellData">Что записывать</param>
        public static void ExcJob(int sheetNum, int columnNum, int rowNum, string path, string cellData)
        {
            FileInfo excTable = new FileInfo(path);

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
                
                ConsoleColors.DrawColor("DarkGray", $"Работаем с колонкой - {columnNum}");

                // Запись в строку
                Variables.rowData = loneSheet.Cells[rowNum + 2, columnNum].Value == null ? "" : loneSheet.Cells[rowNum + 2, columnNum].Value.ToString();

                ConsoleColors.DrawColor("DarkGray", $"Записали данные: {Variables.rowData}, в строку - {rowNum}");

            }
        }
    }
}
