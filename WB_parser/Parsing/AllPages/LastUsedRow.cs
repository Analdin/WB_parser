using OfficeOpenXml;

namespace WB_parser.Parsing.AllPages
{
    public class LastUsedRow
    {
        public static int GetLastUsedRow(ExcelWorksheet sheet, int colNum)
        {
            var row = sheet.Dimension.End.Row;
            while(row >= 1)
            {
                var range = sheet.Cells[row, colNum, row, sheet.Dimension.End.Row];
                if(range.Any(c => !string.IsNullOrEmpty(c.Text)))
                {
                    break;
                }
                row--;
            }
            return row;
        }
    }
}
