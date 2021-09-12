using log4net;
using log4net.Core;
using SuperSocket.Common;
using Server.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Websocket.Temp;
using System.Threading.Tasks;
using Server.Websocket.AsyncWebsocket;

namespace Server.Execute
{
    /// <summary>
    /// 单例
    /// </summary>
    public partial class ExecuteTrigger
    {
        private static ExecuteTrigger _instance;
        public static ExecuteTrigger Instance
        {
            get
            {
                if (_instance == null) _instance = new ExecuteTrigger();
                return _instance;
            }
        }

        private Dictionary<string, Type> _typeDic = new Dictionary<string, Type>();

        private ILog _logger = LogManager.GetLogger(typeof(ExecuteTrigger));

        private ExecuteTrigger()
        {
            // 初始化 Command
            //这里找出了实现 IWebsocketCommand 接口的所有类
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IWebsocketCommand)) && !t.IsAbstract))
                                .ToList();
            types.ForEach(t =>
            {
                //创建一个obj对象
                if (Activator.CreateInstance(t) is IWebsocketCommand command && !_typeDic.ContainsKey(command.Name))
                {
                    _typeDic.Add(command.Name, t);
                }
            });
        }


        public void Execute(ReceivedMessage message)
        {
            // 开始触发
            _logger.Info("开始响应命令: " + message.Body.Command);
            try
            {
                // 判断是有task需要触发
                if (!string.IsNullOrEmpty(message.Body.taskId))
                {
                    // 从静态配置中查找
                    if (!SendCallback.Insance.TryGetValue(message.Body.taskId, out CallbackOption<ReceivedMessage> callback)) return;

                    // 雇用回调
                    callback.TaskCompletion.SetResult(message);

                    // 移除回调
                    SendCallback.Insance.Remove(message.Body.taskId);

                    return;
                }

                if (_typeDic.Count == 0)
                {
                    // 发送错误信息
                    // 回复
                    Response response = new Response(message.Body)
                    {
                        result = "没有找到实现 IWebsocketCommand 的类"
                    };
                    message.Session.Send(response.SerializeObject());
                    return;
                }

                // 从程序集中读取实现了 ICommand 接口的类
                if (_typeDic.TryGetValue(message.Body.name, out Type value))
                {
                    IWebsocketCommand command = Activator.CreateInstance(value) as IWebsocketCommand;//创建一个obj对象
                    command.Logger = message.Session.Logger;

                    command.ExecuteCommand(message);
                    //MethodInfo mi = item.GetMethod("Interface_void");
                    //mi.Invoke(obj, null);//调用方法
                    //mi = item.GetMethod("BaseClass_VoidPublic");
                    //string s = mi.Invoke(obj, null) as string;//调用有返回值的方法，使用 as 关键字 转换返回的类型
                    //Console.WriteLine(item);

                }
                else
                {
                    Response response = new Response(message.Body)
                    {                       
                        status = 204,
                        statusText = string.Format("没有找到名为 {0} 的类", message.Body.name),
                    };

                    message.Session.Send(response.SerializeObject());
                    return;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
            }
        }
    }
}
