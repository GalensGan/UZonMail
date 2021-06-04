using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    /// <summary>
    /// 每个用户的数据
    /// </summary>
    public class PersonalData
    {
        // 发送时间间隔
        public double sendInterval = 5;

        public double sendIntervalRandom = 1.5;

        // 发件人
        public DataTable senders;

        // 收件人
        public DataTable receivers;

        // 变量，每个人对应的值
        public DataTable variablesTable;

        // 上一次使用的模板名称
        public string usedTemplatePath = string.Empty;
    }
}
