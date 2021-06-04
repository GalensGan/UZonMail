using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Helper
{
   public class NPOIHelper
    {
        /// <summary>
        /// 读取单元格的值
        /// </summary>
        /// <param name="index">单元格的索引</param>
        /// <param name="cell">指定单元格</param>
        /// <returns>返回值</returns>
        public static string ReadCellValue(ICell cell, IFormulaEvaluator evalor)
        {
            if (cell == null) return string.Empty;
            string cellValue;
            switch (cell.CellType)
            {
                case CellType.String:
                    cellValue = cell.StringCellValue;
                    break;
                case CellType.Numeric:
                    cellValue = cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                    break;
                case CellType.Boolean:
                    cellValue = cell.BooleanCellValue.ToString();
                    break;
                case CellType.Error:
                    cellValue = cell.ErrorCellValue.ToString();
                    break;
                case CellType.Formula:
                    evalor.EvaluateFormulaCell(cell);
                    cellValue = cell.NumericCellValue.ToString();
                    break;
                default:
                    cellValue = string.Empty;
                    break;
            }
            return cellValue.Trim();
        }
    }
}
