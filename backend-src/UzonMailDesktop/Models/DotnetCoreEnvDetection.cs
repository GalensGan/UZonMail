using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UzonMailDesktop.Models
{
    internal class DotnetCoreEnvDetection : IDetectRuntimeEnv
    {
        public string Name => "ASP.NET Core 8.0";

        public string FailedMessage => "请下载安装 ASP.NET Core 8.0 运行时环境。\n点击确认跳转到下载页";

        public string RedirectUrl => "https://dotnet.microsoft.com/zh-cn/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer";

        public bool DetectEnv()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (output.StartsWith("8."))
                return true;

            return false;
        }
    }
}
