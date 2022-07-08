using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WB_parser.Variable
{
    public class Variables
    {
        public static string category { get; set; }
        public static string subCategory { get; set; }
        public static string priceChoose { get; set; }
        public static string discountSet { get; set; }
        public static string rowData { get; set; }
    }

    public class VariablesForReport
    {
        public static string tovName { get; set; }
        public static string tovPriceWithDiscount { get; set; }
        public static string tovPriceWithoutDiscount { get; set; }
    }
}
