using log4net;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SendMultipleEmails.Pages
{
    class AddVariableViewModel : ScreenChild
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Senders_ImportViewModel));
        public AddVariableViewModel(Store store) : base(store)
        {
            Sheets = new List<string>();
        }

        public string ExcelFullPath { get; set; }

        public IList<string> Sheets { get; set; }

        public string SelectedSheet { get; set; }

        public void AddVariables()
        {
            // 判断文件是否存在
            if (string.IsNullOrWhiteSpace(ExcelFullPath))
            {
                MessageBoxX.Show("请选择需要导入的Excel文件！", "温馨提示");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedSheet))
            {
                MessageBoxX.Show("请选择需要导入的页签！", "温馨提示");
                return;
            }

            // 从 Excel 中读取数据
            try
            {
                using (Stream fs = new FileStream(ExcelFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs);
                    ISheet sheet = workbook.GetSheet(SelectedSheet);

                    IFormulaEvaluator evalor = workbook.GetCreationHelper().CreateFormulaEvaluator();

                    // 行的最后一个值是 index
                    int firstRowNum = sheet.FirstRowNum;
                    int lastRowNum = sheet.LastRowNum;
                    IRow header = sheet.GetRow(firstRowNum);

                    // 列是最后一个值是 count
                    int firstColumnNum = header.FirstCellNum;
                    int lastColumnNum = header.LastCellNum;

                    // 设置数据行
                    _logger.Info("开始个人基本信息变量...");
                    DataTable table = new DataTable("variables");
                    Store.PersonalDataManager.PersonalData.variablesTable = table;

                    List<int> cols = new List<int>();
                    // 读取表头
                    for (int col = firstColumnNum; col < lastColumnNum; col++)
                    {
                        ICell headerCell = header.GetCell(col);
                        string headerCellValue = Helper.NPOIHelper.ReadCellValue(headerCell, evalor);
                        if (string.IsNullOrEmpty(headerCellValue)) continue;
                        // 添加到表头
                        cols.Add(col);
                        table.Columns.Add(headerCellValue);
                    }

                    // 判断是否有Name 或者 Name 列，如果没有，报错
                    List<string> names = Store.PersonalDataManager.GetTableNames(table);
                    if(!names.Contains("Name") && !names.Contains("姓名"))
                    {
                        MessageBoxX.Show("保证数据列中至少有“Name”或者“姓名”列", "格式错误");
                        fs.Close();
                        return;
                    }

                    // 获取其它数据
                    for (int r = firstRowNum + 1; r <= lastRowNum; r++)
                    {
                        IRow dataRow = sheet.GetRow(r);
                        List<string> rowValue = cols.ConvertAll(col =>
                        {
                            ICell cell = dataRow.GetCell(col);
                            string value = Helper.NPOIHelper.ReadCellValue(cell, evalor);
                            return value;
                        });
                        // 全为 0 时，不添加
                        if (rowValue.Where(item => !string.IsNullOrEmpty(item)).Count() == 0) continue;
                        table.Rows.Add(rowValue.ToArray());
                    }
                    Store.PersonalDataManager.Save();

                    _logger.Info("导入完成。");
                    string info = string.Format("共导入{0}条数据",table.Rows.Count);
                    _logger.Info(info);   
                    fs.Close();
                    this.RequestClose(true);

                    MessageBoxX.Show(info, "导入完成");
                }
            }
            catch (IOException io)
            {
                _logger.Error(io.Message, io);
                MessageBoxX.Show(io.Message, "IO异常");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "读取文件异常");
            }
        }

       

        /// <summary>
        /// 读取临时数据表中的数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private string ReadDicData(Dictionary<string, List<string>> dic, string key, int index)
        {
            if (dic.TryGetValue(key, out List<string> list))
            {
                if (list.Count <= index) return string.Empty;
                return list[index];
            }
            return string.Empty;
        }

        public void Quite()
        {
            this.RequestClose(false);
        }

        public void SelectFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                CheckPathExists = true,
                DefaultExt = ".xlsx",
                Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                // 读取 Excel 中的 sheet
                try
                {
                    SelectedSheet = "";
                    Sheets.Clear();
                    using (Stream stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        XSSFWorkbook workbook = new XSSFWorkbook(stream);
                        for (int i = 0; i < workbook.Count; i++)
                        {
                            Sheets.Add(workbook.GetSheetName(i));
                        }
                        if (Sheets.Count > 0)
                        {
                            SelectedSheet = Sheets[0];
                        }
                        stream.Close();
                    }

                    ExcelFullPath = openFileDialog.FileName;
                }
                catch (IOException ex)
                {
                    _logger.Error(ex.Message, ex);
                    MessageBoxX.Show(ex.Message, "读取错误", null, MessageBoxButton.OK, new MessageBoxXConfigurations() { MessageBoxIcon = MessageBoxIcon.Error });
                }
            }
        }
    }
}
