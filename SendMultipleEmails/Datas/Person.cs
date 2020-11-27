using log4net;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class Person
    {
        [LiteDB.BsonId]
        public int Id { get; set; }
        /// <summary>
        /// 用于排序
        /// </summary>
        public int Order { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } = string.Empty;

        public virtual bool Validate(ILog logger)
        {
            if (string.IsNullOrEmpty(Name))
            {
                if (logger == null) MessageBoxX.Show("姓名不能为空", "温馨提示");
                else logger.Warn(string.Format("第[{0}]条记录的姓名为空，姓名：{1}，邮箱：{2}", Order, Name, Email));
                return false;
            }

            if (string.IsNullOrEmpty(Email))
            {
                if(logger==null)MessageBoxX.Show("邮箱不能为空", "温馨提示");
                else logger.Warn(string.Format("第[{0}]条记录的邮箱为空，姓名：{1}，邮箱：{2}", Order, Name, Email));
                return false;
            }

            // 验证邮箱格式
            // "^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"
            Regex regex = new Regex(@"^\w+@\w{2,5}.com");
            if (!regex.IsMatch(Email))
            {
                if(logger==null) MessageBoxX.Show("邮箱格式不正确", "温馨提示");
                else logger.Warn(string.Format("第[{0}]条记录的邮箱格式错误，姓名：{1}，邮箱：{2}", Order, Name, Email));
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if(obj is Person p)
            {
                return p.Email.Equals(this.Email);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Email.GetHashCode();
        }
    }
}
