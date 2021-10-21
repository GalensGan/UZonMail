using Server.Execute;
using Server.Protocol;
using SuperSocket.SocketBase.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Websocket.Commands
{
    class SelectFiles : IWebsocketCommand
    {
        public ILog Logger { get; set; }
        public string Name => CommandClassName.SelectFiles.ToString();

        public void ExecuteCommand(ReceivedMessage message)
        {
            // 打开文件选择框
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Multiselect = true,
                Title = "请选择附件",
                Filter = "所有文件（*.*）|*.*"
            };
            var results = System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 返回数据
                    return ofd.FileNames.ToList();
                }
                // 发送成功
                return new List<string>();
            });

            message.Response(new Response(message.Body)
            {
                ignoreError = true,
                result = results,
                status = results.Count > 0 ? 200 : 204,
            });
        }
    }
}
