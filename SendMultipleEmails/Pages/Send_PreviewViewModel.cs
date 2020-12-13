using GalensSDK.StyletEx;
using log4net;
using Newtonsoft.Json.Bson;
using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using SendMultipleEmails.Pages;
using Stylet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    public class Send_PreviewViewModel : ScreenChild
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendViewModel));
        public Send_PreviewViewModel(Store store) : base(store) { }



        private Microsoft.Web.WebView2.Wpf.WebView2 _webView;
        private string _contentHtml = string.Empty;
        public string ContentHtml
        {
            get => _contentHtml;
            set
            {
                base.SetAndNotify(ref _contentHtml, value);

                if (_webView != null)
                {
                    Execute.OnUIThreadSync(new Action(() =>
                    {
                        _webView.NavigateToString(value);
                    }));
                }
            }
        }

        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            Send_PreviewView view = (Send_PreviewView)this.View;
            _webView = view.webView;
            EnsureCoreWebView2Async();
        }

        private async void EnsureCoreWebView2Async()
        {
            await _webView.EnsureCoreWebView2Async();
            // 读取第一个数据
            Tuple<Person, string> data = Store.QueueReceivers.FirstOrDefault();
            if (data != null && _webView != null)
            {
                Execute.OnUIThreadSync(new Action(() =>
                {
                    _webView.NavigateToString(data.Item2);
                }));
            }
        }

        private List<string> Contents;
        private Thread _thread;

        public override void AfterInvoke(InvokeParameter parameter)
        {
            Store.QueueReceivers = new ConcurrentQueue<Tuple<Person, string>>();
            Contents = new List<string>();
            _previewIndex = 0;

            // 开辟线程进行读取
            _thread = new Thread(new ThreadStart(ReadPostData));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void ReadPostData()
        {
            // 读取模板内容
            string content = Store.TemplateManager.GetTemplate(Store.TemplateManager.TemplateFiles);

            // 查找模板中有哪些需要替换的数据
            char[] chars = content.ToCharArray();
            int startIndex = -1;
            int endIndex = -1;
            List<string> variables = new List<string>();

            // 因为是双括号，所以从1开始，防止从0开始索引为负
            for (int i = 1; i < chars.Length; i++)
            {
                if (chars[i] == '{' && chars[i - 1] == '{')
                {
                    startIndex = i + 1;
                }
                else if (chars[i] == '}' && chars[i - 1] == '}')
                {
                    endIndex = i - 2;
                    // 已经成对，取出
                    if (startIndex > 0 && endIndex > 0)
                    {
                        string variable = content.Substring(startIndex, endIndex - startIndex + 1);
                        variables.Add(variable);
                        startIndex = -1;
                        endIndex = -1;
                    }
                }
            }
            variables = variables.Distinct().ToList().ConvertAll(item => item.Trim());

            // 对比提供的个人数据中是否存在
            List<string> tableNames = Store.PersonalDataManager.GetTableNames(Store.PersonalDataManager.PersonalData.variablesTable);

            for (int i = variables.Count - 1; i > -1; i--)
            {
                if (!tableNames.Contains(variables[i]))
                {
                    // 记录到日志中
                    _logger.Info(string.Format("模板中定义的变量:[{0}]没有找到，不进行替换", variables[i]));
                    variables.RemoveAt(i);
                }
            }

            DataRow[] receivers = Store.PersonalDataManager.GetCurrentReceiver();
            // 判断变量中存在的是 Name 还是 姓名
            string keyColumn = string.Empty;
            if (tableNames.Contains("Name"))
            {
                keyColumn = "Name";
            }
            else if (tableNames.Contains("姓名"))
            {
                keyColumn = "姓名";
            }
            if (string.IsNullOrEmpty(keyColumn))
            {
                // 未找到姓名
                _logger.Info("未找到姓名（Name）列，退出");
                Execute.OnUIThreadSync(new Action(() =>
                {
                    MessageBoxX.Show("在数据中未找到“Name”或者“姓名”列。", "格式错误");
                    InvokeTo(new InvokeParameter() { InvokeId = InvokeID.Send_Preview.ToString() });
                    return;
                })); 
            }

            foreach (DataRow receiverRow in receivers)
            {
                Person receiver = new Person()
                {
                    UserId = receiverRow["Name"].ToString(),
                    Email = receiverRow["Email"].ToString(),
                };

                // 通过名字找到对应收件人数据

                // 按姓名对应，找到发件人
                DataRow[] receiverDataRows = Store.PersonalDataManager.PersonalData.variablesTable.Select(string.Format("{0}='{1}'", keyColumn, receiver.UserId));
                string result = content;
                if (receiverDataRows.Length != 1)
                {
                    _logger.Info(string.Format("未找到发件人:{0}相对应的参数", receiver.UserId));
                }
                else
                {
                    DataRow receiverData = receiverDataRows[0];
                    // 将模板里面的字符替换成用户数据               
                    foreach (string str in variables)
                    {
                        Regex regex = new Regex(@"\{\{\s*" + str + @"\s*\}\}");
                        result = regex.Replace(result, receiverData[str].ToString());
                    }
                }

                // 保存到队列
                Store.QueueReceivers.Enqueue(new Tuple<Person, string>(receiver, result));
                // 保存到显示的数组
                Contents.Add(result);

                if (Store.QueueReceivers.Count == 1)
                {
                    // 显示到界面上
                    ContentHtml = result;
                }
            }

            // 结束后显示序号
            CurrentIndex = string.Format("第{0}个/共{1}个", _previewIndex + 1, Contents.Count);
        }

        private int _previewIndex = 0;

        // 开始发送
        public void Send()
        {
            if (string.IsNullOrEmpty(MailTitle))
            {
                MessageBoxX.Show("请输入主题", "主题为空");
                return;
            }
            Store.MainTitle = MailTitle;
            InvokeTo(new InvokeParameter() { InvokeId = InvokeID.Send_Sending.ToString()});
        }

        public string CurrentIndex { get; set; }

        public void Previous()
        {
            _previewIndex--;
            if (_previewIndex < 0)
            {
                _previewIndex += Contents.Count;
            }
            ContentHtml = this.Contents[_previewIndex];
            CurrentIndex = string.Format("第{0}个/共{1}个", _previewIndex + 1, Contents.Count);
        }

        public void Next()
        {
            _previewIndex++;
            _previewIndex %= Contents.Count;
            ContentHtml = this.Contents[_previewIndex];
            CurrentIndex = string.Format("第{0}个/共{1}个", _previewIndex + 1, Contents.Count);
        }


        public void Back()
        {
            // 如果线程还在运行，立即停止            
            _thread.Abort();
            Thread.Sleep(50);

            // 清空发送数据组
            Contents = null;
            Store.QueueReceivers = null;

            // 返回
            InvokeTo(new InvokeParameter() { InvokeId = InvokeID.Send_New.ToString()});
        }

        public string MailTitle { get; set; } = string.Empty;
    }
}
