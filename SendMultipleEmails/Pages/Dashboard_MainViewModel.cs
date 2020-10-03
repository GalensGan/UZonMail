using LiveCharts;
using LiveCharts.Wpf;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    class Dashboard_MainViewModel : ScreenChild
    {
        public Dashboard_MainViewModel(Store store) : base(store) { }

        protected override void OnActivate()
        {
            base.OnActivate();

            // 计算发送总数
            SentTotal = Store.HistoryManager.HistoryTable.Rows.Count;

            // 计算上一次的发送数量
            LastSentTotal = Store.HistoryManager.Select(string.Format("{0}={1}", History.GroupId.ToString(), Store.HistoryManager.Index)).Length;

            // 填充发件箱类型数量表
            SeriesCollection senderChart = new SeriesCollection();
            // 获取发件箱类型
            Dictionary<string, int> senderCountDic = new Dictionary<string, int>();
            string pattern = "@.+\\.com$";
            foreach(DataRow row in Store.PersonalDataManager.PersonalData.senders.Rows)
            {
                string email = row["Email"].ToString();
                // 通过正则表达式，获取邮箱
                Match m = Regex.Match(email, pattern);
                if (m.Success)
                {
                    // 添加到字典
                    string key = m.Groups[0].Value;
                    if (senderCountDic.ContainsKey(key)){
                        senderCountDic[key] += 1;
                    }
                    else
                    {
                        senderCountDic.Add(key, 1);
                    }
                }
            }
            foreach(KeyValuePair<string,int> kv in senderCountDic)
            {
                senderChart.Add(
                new PieSeries
                {
                    Title = kv.Key,
                    Values = new ChartValues<int> { kv.Value },
                    PushOut = 3,
                    DataLabels = true,
                    LabelPoint = _pointLabel,
                });
            }
            SenderChart = senderChart;

            // 填充收件箱类型数量表
            SeriesCollection receiverChart = new SeriesCollection();
            // 获取收件箱类型
            Dictionary<string, int> receiverCountDic = new Dictionary<string, int>();
            foreach (DataRow row in Store.PersonalDataManager.PersonalData.receivers.Rows)
            {
                string email = row["Email"].ToString();
                // 通过正则表达式，获取邮箱
                Match m = Regex.Match(email, pattern);
                if (m.Success)
                {
                    // 添加到字典
                    string key = m.Groups[0].Value;
                    if (receiverCountDic.ContainsKey(key))
                    {
                        receiverCountDic[key] += 1;
                    }
                    else
                    {
                        receiverCountDic.Add(key, 1);
                    }
                }
            }
            foreach (KeyValuePair<string, int> kv in receiverCountDic)
            {
                receiverChart.Add(
                new PieSeries
                {
                    Title = kv.Key,
                    Values = new ChartValues<int> { kv.Value },
                    DataLabels = true,
                });
            }
            ReceiverChart = receiverChart;
        }
        

        public int SentTotal { get; set; }

        public int LastSentTotal { get; set; }

        #region 发件箱图表
        private Func<ChartPoint, string> _pointLabel = chartPoint => string.Format("{0} ({1}%)", chartPoint.Y, Math.Round(chartPoint.Participation * 100, 1));

        public SeriesCollection SenderChart { get; set; }
        #endregion

        #region 收件箱图表
        public SeriesCollection ReceiverChart { get; set; }
        #endregion
    }
}
