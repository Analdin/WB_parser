using OfficeOpenXml;
using WB_parser.Color;
using WB_parser.Variable;
using WB_parser.Parsing.AllPages;

namespace WB_parser.ExcelJob
{
    public class JobWithExcel
    {
        public static int FirstPrice { get; set; }
        public static int LastPrice { get; set; }

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
                int sheetsCount = exclPack.Workbook.Worksheets.Count;
                ConsoleColors.DrawColor("DarkGray", $"Количество листов в таблице с отчетом - {sheetsCount}");
                ExcelWorksheet loneSheet = exclPack.Workbook.Worksheets["Отчёт"];
                int iRowCnt = LastUsedRow.GetLastUsedRow(loneSheet, columnNum);

                ConsoleColors.DrawColor("Cyan", $"Количество строк {iRowCnt}");
                rowNum = iRowCnt + 1;

                // Запись в строку
                loneSheet.Cells[rowNum, columnNum].Value = Variables.rowData;

                // Проверка на условия пользователя
                ConsoleColors.DrawColor("Cyan", $"Проверяем на соответствие условиям, полученное значение {Variables.rowData}");

                var split = Variables.priceChoose.Split('-');
                FirstPrice = Convert.ToInt32(split[0].Trim());
                LastPrice = Convert.ToInt32(split[1].Trim());

                // Условие: диапазон цен
                if (String.IsNullOrWhiteSpace(Variables.priceChoose))
                {
                    ConsoleColors.DrawColor("Gray", $"Диапазон цены не задан");
                }
                else if(Convert.ToInt32(Variables.rowData) >= FirstPrice && Convert.ToInt32(Variables.rowData) <= LastPrice)
                {
                    ConsoleColors.DrawColor("Cyan", $"Сохраняем цены только в диапазоне: {FirstPrice} - {LastPrice}");
                }

                exclPack.Save();

                ConsoleColors.DrawColor("DarkGray", $"Записали данные: {Variables.rowData}, в строку - {rowNum} и колонку {columnNum}");
            }
        }
    }
}
