using Newtonsoft.Json.Linq;
using Server.Config;
using Server.Database;
using Server.Database.Models;
using Server.Http.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Server.Http.Modules.SendEmail
{
    /// <summary>
    /// 邮箱预览
    /// 要实现用户级单例模式
    /// </summary>
    class EmailPreview
    {

        /// <summary>
        /// 创建预览
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subject"></param>
        /// <param name="receivers"></param>
        /// <param name="data"></param>
        /// <param name="templateId"></param>
        /// <param name="liteDb"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool CreateEmailPreview(string userId, string subject, JArray receivers, JArray data, string templateId, LiteDBManager liteDb, out string message)
        {
            EmailPreview temp = new EmailPreview(subject, receivers, data, templateId, liteDb);

            // 保存到全局
            InstanceCenter.EmailPreview.Upsert(userId, temp);

            message = "success";
            return true;
        }


        protected string Subject { get; private set; }
        protected JArray Receivers { get; private set; }
        protected JArray Data { get; private set; }
        protected Template Template { get; private set; }
        protected LiteDBManager LiteDb { get; private set; }

        private List<SendItem> _sendItems;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="receivers"></param>
        /// <param name="data"></param>
        /// <param name="templateId"></param>
        /// <param name="liteDb"></param>
        protected EmailPreview(string subject, JArray receivers, JArray data, string templateId, LiteDBManager liteDb)
        {
            Subject = subject;
            Receivers = receivers;
            Data = data;
            LiteDb = liteDb;

            // 获取模板
            Template = LiteDb.SingleOrDefault<Template>(t => t._id == templateId);
        }

        /// <summary>
        /// 生成数据
        /// </summary>
        public virtual GenerateInfo Generate()
        {
            // 生成发件模板，用异步，否则会阻塞该线程
            new Task(() => GenerateSendItems()).Start();

            return new GenerateInfo()
            {
                ok = true
            };
        }

        /// <summary>
        /// 创建预览项
        /// </summary>
        protected void GenerateSendItems()
        {
            // 生成每一项
            _sendItems = new List<SendItem>();
            List<ReceiveBox> receiveBoxes = new List<ReceiveBox>();

            if (Receivers != null && Receivers.Count > 0)
            {
                // 获取当前收件人或组下的所有人
                foreach (JToken jt in Receivers)
                {
                    // 判断 type
                    string type = jt.Value<string>(Fields.type_);
                    string id = jt.Value<string>(Fields._id);
                    if (type == Fields.group)
                    {
                        // 找到group下所有的用户
                        List<ReceiveBox> boxes = LiteDb.Fetch<ReceiveBox>(r => r.groupId == id);

                        // 如果没有，才添加
                        foreach (ReceiveBox box in boxes)
                        {
                            if (receiveBoxes.Find(item => item._id == box._id) == null) receiveBoxes.Add(box);
                        }
                    }
                    else
                    {
                        // 选择了单个用户
                        var box = LiteDb.SingleOrDefault<ReceiveBox>(r => r._id == id);
                        if (box != null && receiveBoxes.Find(item => item._id == box._id) == null) receiveBoxes.Add(box);
                    }
                }
            }
            else if (Data != null && Data.Count > 0)
            {
                // 从数据中获取收件人
                foreach (JToken jt in Data)
                {
                    string userName = jt.Value<string>(Fields.userName);
                    if (string.IsNullOrEmpty(userName)) continue;

                    // 从数据库中查找
                    var receiver = LiteDb.FirstOrDefault<ReceiveBox>(r => r.userName == userName);
                    if (receiver != null) receiveBoxes.Add(receiver);
                }
            }


            // 开始添加                
            foreach (var re in receiveBoxes)
            {

                var item = new SendItem()
                {
                    receiverName = re.userName,
                    receiverEmail = re.email,
                };

                string sendHtml = Template.html;
                string subjectTemp = Subject;

                // 处理模板数据

                // 判断有没有数据
                JObject itemObj = null;
                if (Data == null || Data.Count < 1) itemObj = new JObject();
                else
                {
                    if (Data.FirstOrDefault(jt => jt.Value<string>(Fields.userName) == re.userName) is JObject itemDataTemp) itemObj = itemDataTemp;
                }

                // 添加默认用户名
                if (!itemObj.ContainsKey(Fields.userName))
                {
                    itemObj.Add(new JProperty(Fields.userName, re.userName));
                }

                // 添加收件箱
                if (!itemObj.ContainsKey(Fields.inbox))
                {
                    itemObj.Add(new JProperty(Fields.inbox, re.email));
                }

                // 获取数据
                List<string> keys = itemObj.Properties().ToList().ConvertAll(p => p.Name);

                // 判断是否有自定义内容
                if (keys.Contains(Fields.body))
                {
                    // 获取 body 值
                    string body = itemObj.Value<string>(Fields.body);
                    if (!string.IsNullOrEmpty(body))
                    {
                        sendHtml = body;
                    }
                }
                // 判断是否有自定义模板
                else if (keys.Contains(Fields.template))
                {
                    string customTemplateName = itemObj.Value<string>(Fields.template);
                    if (!string.IsNullOrEmpty(customTemplateName))
                    {
                        // 获取新模板，如果失败，则跳过，不发送
                        var customTemplate = LiteDb.SingleOrDefault<Template>(t => t.name == customTemplateName);
                        if (customTemplate != null)
                        {
                            sendHtml = customTemplate.html;
                        }
                    }
                }

                // 替换模板内数据                    
                foreach (string key in keys)
                {
                    var regex = new Regex("{{\\s*" + key + "\\s*}}");
                    sendHtml = regex.Replace(sendHtml, itemObj[key].Value<string>());

                    // 同时替换主题数据
                    subjectTemp = regex.Replace(subjectTemp, itemObj[key].Value<string>());
                }



                item.html = sendHtml;
                item.subject = subjectTemp;

                // 添加到保存的集合中
                _sendItems.Add(item);
            };

            // 添加序号
            for (int i = 0; i < _sendItems.Count; i++)
            {
                _sendItems[i].index = i;
                _sendItems[i].total = _sendItems.Count;
            }

            PreviewItemCreated(_sendItems, receiveBoxes);
        }

        protected virtual void PreviewItemCreated(List<SendItem> sendItems, List<ReceiveBox> receiveBoxes)
        {

        }

        private int _index;
        // 获取预览内容
        public SendItem GetPreviewHtml(string directive)
        {
            if (_sendItems.Count < 1) return null;

            switch (directive)
            {
                case "first":
                    _index = 0;
                    return _sendItems.FirstOrDefault();
                case "next":
                    _index++;
                    return _sendItems[CycleInt(_index, _sendItems.Count)];
                case "previous":
                    _index--;
                    return _sendItems[CycleInt(_index, _sendItems.Count)];
                default:
                    // 通过名字来搜索
                    return null;
            }
        }

        private int CycleInt(int index, int total)
        {
            int result = index % total;
            if (result >= 0) return result;
            return total + result;
        }
    }
}
