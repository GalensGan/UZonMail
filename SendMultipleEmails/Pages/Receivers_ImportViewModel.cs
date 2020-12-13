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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Group = SendMultipleEmails.Datas.Group;

namespace SendMultipleEmails.Pages
{
    class Receivers_ImportViewModel : ScreenChild
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Receivers_ImportViewModel));
        public Receivers_ImportViewModel(Store store) : base(store)
        {
            Sheets = new List<string>();
        }

        public string ExcelFullPath { get; set; }

        public IList<string> Sheets { get; set; }

        public string SelectedSheet { get; set; }

        public void AddReceivers()
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
                    _logger.Info("开始导入收件人信息...");
                    Dictionary<string, List<string>> tableData = new Dictionary<string, List<string>>();

                    for (int col = firstColumnNum; col < lastColumnNum; col++)
                    {
                        List<string> columnData = new List<string>();
                        for (int r = firstRowNum; r <= lastRowNum; r++)
                        {
                            // 第一行为表头
                            if (r == firstRowNum)
                            {
                                // 读取表头                
                                ICell headerCell = header.GetCell(col);
                                string cellValue = Helper.NPOIHelper.ReadCellValue(headerCell, evaluator);
                                if (string.IsNullOrEmpty(cellValue)) continue;
                                if (!tableData.ContainsKey(cellValue) && (cellValue == "姓名" || cellValue == "邮箱" || cellValue == "组"))
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

                    // 从数据库中读取组的信息
                    List<Group> groups = Store.GetUserDatabase<IGroup>().GetAllGroups().ToList();
                    // 获取组的全称
                    groups.ForEach(item => item.GenerateFullName(groups));

                    // 对每个数据进行组装
                    for (int i = 0; i < totalCount; i++)
                    {
                        // 获取
                        Receiver receiver = new Receiver()
                        {
                            UserId = ReadDicData(tableData, "姓名", i),
                            Email = ReadDicData(tableData, "邮箱", i),
                            GroupId = Group.GetGroupIdByFullName(groups,Store,ReadDicData(tableData, "组", i)),
                            Order = i,
                        };

                        if (!receiver.Validate(_logger)) continue;

                        // 查找，判断是否重复
                        Receiver existReciver = Store.GetUserDatabase<IReceiverDb>().FindOneReceiverByEmail(receiver.Email);
                        if (existReciver != null)
                        {
                            _logger.Info(string.Format("第[{0}]行，[{1}] 导入失败！原因：发件箱已经存在", existReciver.Order, existReciver.UserId));
                            continue;
                        }

                        // 添加发件人
                        Store.GetUserDatabase<IReceiverDb>().InsertReceiver(receiver);

                        successNum++;
                        _logger.Info(string.Format("第[{0}]行，[{1}] 导入成功！", receiver.Order, receiver.UserId));
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

                    Store.ShowInfo(info, "导入完成");
                }
            }
            catch (IOException io)
            {
                _logger.Error(io.Message, io);
                Store.ShowError(io.Message, "IO异常");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
               Store.ShowError(ex.Message, "读取文件异常");
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
                    MessageBoxX.Show(Store.MainWindow,ex.Message, "读取错误", MessageBoxButton.OK,MessageBoxIcon.Error);
                }
            }
        }
    }
}
