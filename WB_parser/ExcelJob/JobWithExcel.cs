using OfficeOpenXml;
using WB_parser.Color;
using WB_parser.Variable;
using WB_parser.Parsing.AllPages;

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
        public static void ExcJob(int sheetNum, int columnNum, string path, string cellData)
        {
            int rowNum = 1;

            FileInfo excTable = new FileInfo(path);

            ConsoleColors.DrawColor("DarkGray", $"Путь до таблицы - {path}");

            Variables.rowData = cellData;

            if(String.IsNullOrWhiteSpace(cellData))
            {
                ConsoleColors.DrawColor("Red", $"Что-то должно быть записано в cellData... Сейчас она пуста.");
                //throw new Exception();
            }
            else
            {
                ConsoleColors.DrawColor("Gray", $"Записанная информация в cellData - {cellData}");
            }

            using (ExcelPackage exclPack = new ExcelPackage(excTable))
            {
                // Количество строк
                int sheetsCount = exclPack.Workbook.Worksheets.Count;
                ConsoleColors.DrawColor("DarkGray", $"Количество листов в таблице с отчетом - {sheetsCount}");

                // Берем лист по номеру
                ExcelWorksheet loneSheet = exclPack.Workbook.Worksheets["Отчёт"];

                // Количество строк
                //int iRowCnt = loneSheet.Cells.Where(cell => !cell.Value.ToString().Equals("")).Last().End.Row;

                int iRowCnt = LastUsedRow.GetLastUsedRow(loneSheet, columnNum);

                ConsoleColors.DrawColor("Cyan", $"Количество строк {iRowCnt}");
                rowNum = iRowCnt + 1;

                // Запись в строку
                loneSheet.Cells[rowNum, columnNum].Value = Variables.rowData;

                exclPack.Save();

                ConsoleColors.DrawColor("DarkGray", $"Записали данные: {Variables.rowData}, в строку - {rowNum} и колонку {columnNum}");
            }
        }
    }
}
