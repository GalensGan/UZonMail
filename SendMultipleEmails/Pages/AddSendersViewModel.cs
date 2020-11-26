using log4net;
using Microsoft.Win32;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace SendMultipleEmails.Pages
{
    class AddSendersViewModel : ScreenChild
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(AddSendersViewModel));
        public AddSendersViewModel(Store store) : base(store)
        {
            Sheets = new List<string>();
        }

        public string ExcelFullPath { get; set; }

        public IList<string> Sheets { get; set; }

        public string SelectedSheet { get; set; }

        public void AddSenders()
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
                    IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

                    // 行号从 0 开始
                    int firstRowNum = sheet.FirstRowNum;
                    int lastRowNum = sheet.LastRowNum;
                    IRow header = sheet.GetRow(firstRowNum);

                    // 列是序号从 1 开始
                    int firstColumnNum = header.FirstCellNum;
                    int lastColumnNum = header.LastCellNum;

                    int totalCount = lastRowNum - firstRowNum;
                    int successNum = 0;

                    // 设置数据行
                    _logger.Info("开始导入发件人信息...");
                    Dictionary<string, List<string>> tableData = new Dictionary<string, List<string>>();
                    for (int col = firstColumnNum; col <lastColumnNum; col++)
                    {
                        List<string> columnData = new List<string>();
                        for (int r = firstRowNum; r <=lastRowNum; r++)
                        {
                            // 第一行为表头
                            if (r == firstRowNum)
                            {
                                // 读取表头                
                                ICell headerCell = header.GetCell(col);
                                string cellValue = Helper.NPOIHelper.ReadCellValue(headerCell, evaluator);
                                if (string.IsNullOrEmpty(cellValue)) continue;
                                if (!tableData.ContainsKey(cellValue))
                                {
                                    tableData.Add(cellValue, columnData);
                                }
                                continue;
                            }

                            // 读取行中的其它数据
                            IRow dataRow = sheet.GetRow(r);
                            ICell cell = dataRow.GetCell(col);
                            string value = Helper.NPOIHelper.ReadCellValue(cell, evaluator);
                            columnData.Add(value);
                        }
                    }

                    // 对每个数据进行组装
                    for (int i = 0; i <totalCount; i++)
                    {
                        Sender person = new Sender()
                        {
                            Name = ReadDicData(tableData, "姓名", i),
                            Email = ReadDicData(tableData, "邮箱", i),
                            Password = ReadDicData(tableData, "密码", i),
                            SMTP = ReadDicData(tableData, "SMTP", i),
                            Order = i + 1,
                        };

                        if (!person.Validate(_logger)) continue;

                        // 查找，判断是否重复
                        Sender existSender = Store.GetUserDatabase<ISenderDb>().FindOneSenderByEmail(person.Email);
                        if (existSender != null)
                        {
                            _logger.Info(string.Format("第[{0}]行，[{1}] 导入失败！原因：发件箱已经存在", person.Order, person.Name));
                            continue;
                        }

                        // 添加发件人
                        Store.GetUserDatabase<ISenderDb>().InsertSender(person);
                        successNum++;
                        _logger.Info(string.Format("第[{0}]行，[{1}] 导入成功！", person.Order, person.Name));
                    }

                    _logger.Info("导入完成。");
                    string info = string.Format("共导入{0}条数据：成功{1}条，失败{2}条。",
                        totalCount, successNum, totalCount - successNum);
                    _logger.Info(info);
                    if (successNum != totalCount)
                    {
                        info += "\n详见 \\Log\\log.txt";
                    }
                    fs.Close();
                    this.RequestClose(true);

                    MessageBoxX.Show(info,"导入完成");                    
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
                MessageBoxX.Show(ex.Message, "读取文件异常");
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
                    using (Stream stream = new FileStream(openFileDialog.FileName,FileMode.Open, FileAccess.Read, FileShare.Read))
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
