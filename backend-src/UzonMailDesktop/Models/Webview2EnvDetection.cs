using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UZonMailDesktop.Helpers;

namespace UZonMailDesktop.Models
{
    internal class Webview2EnvDetection : IDetectRuntimeEnv
    {
        public string Name => "WebView2";

        public string FailedMessage => "未检测到 Webview2 环境，请下载安装该环境。\n点击确认跳转到下载链接";

        public string RedirectUrl => "https://developer.microsoft.com/zh-cn/microsoft-edge/webview2?form=MA13LH#download";

        public bool DetectEnv()
        {
            // 验证 webview2 环境
            return Webview2Helper.HasWebView2();
        }
    }
}
