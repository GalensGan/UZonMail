using LiteDB;
using log4net;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Panuon.UI.Silver;
using Panuon.UI.Silver.Core;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
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
                    _logger.Info("开始读取个人基本信息变量...");

                    Dictionary<int, string> cols = new Dictionary<int, string>();
                    // 读取表头
                    for (int col = firstColumnNum; col < lastColumnNum; col++)
                    {
                        ICell headerCell = header.GetCell(col);
                        string headerCellValue = Helper.NPOIHelper.ReadCellValue(headerCell, evalor);
                        if (string.IsNullOrEmpty(headerCellValue)) continue;

                        // 对表头去重
                        if (cols.ContainsValue(headerCellValue)) continue;

                        // 添加到表头
                        cols.Add(col,headerCellValue);
                    }

                    // 判断是否有Name 或者 Name 列，如果没有，报错
                    if(!cols.ContainsValue("UserId"))
                    {
                        _logger.Info("数据格式错误，头部没有【UserId】列");
                        MessageBoxX.Show(Store.MainWindow,"保证列头部至少有【UserId】列", "格式错误",MessageBoxIcon.Error);
                        fs.Close();
                        return;
                    }

                    List<BsonDocument> docs = new List<BsonDocument>();
                    // 获取其它数据
                    for (int r = firstRowNum + 1; r <= lastRowNum; r++)
                    {
                        IRow dataRow = sheet.GetRow(r);
                        Dictionary<string, string> rowTemp = new Dictionary<string, string>();
                        foreach(KeyValuePair<int,string> kv in cols)
                        {
                            ICell cell = dataRow.GetCell(kv.Key);
                            string value = Helper.NPOIHelper.ReadCellValue(cell, evalor);
                            rowTemp.Add(kv.Value, value);
                        }

                        // 全为 0 时，不添加
                        if (rowTemp.Values.Where(item => !string.IsNullOrEmpty(item)).Count() == 0)
                        {
                            _logger.InfoFormat("数据行【{0}】全为空，不添加",r);
                            continue;
                        }

                        // 生成bsonDoc
                        BsonDocument doc = new BsonDocument();
                        foreach(KeyValuePair<string,string> kv in rowTemp)
                        {
                            doc[kv.Key] = kv.Value;
                        }

                        // 添加当前用户的userName用来分组
                        doc["$owner"] = Store.CurrentAccount.UserId;
                        // 添加标记为当前值
                        doc["$group"] = "当前-" + Store.CurrentAccount.UserId;
{}
                        docs.Add(doc);
                    }

                    // 删除原来的数据
                    Store.GetCollection(DatabaseName.Variable.ToString()).DeleteMany(new BsonExpression())

                    // 保存进数据库
                    Store.GetCollection(DatabaseName.Variable.ToString()).InsertBulk(docs);

                    _logger.Info("导入完成。");
                    string info = string.Format("共导入{0}条数据",docs.Count);
                    _logger.Info(info);   
                    fs.Close();
                    this.RequestClose(true);
                    MessageBoxX.Show(Store.MainWindow,info, "导入完成",MessageBoxIcon.Success);
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
                    MessageBoxX.Show(Store.MainWindow,ex.Message, "读取错误", MessageBoxButton.OK,MessageBoxIcon.Error);
                }
            }
        }
    }
}
