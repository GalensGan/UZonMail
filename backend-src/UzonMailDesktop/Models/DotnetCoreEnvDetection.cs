using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UZonMailDesktop.Models
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
                    Arguments = "--list-runtimes",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            string[] outputs = output.Split('\n');

            if (outputs.Any(x=>x.StartsWith("Microsoft.AspNetCore.App 8.")))
                return true;

            // 若没有时，判断路径
            string dotnetDir = "C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.App";
            // 获取子文件夹名
            var dirs = System.IO.Directory.GetDirectories(dotnetDir);
            if (dirs.Length == 0)
                return false;

            // 转成版本号
            var versions = dirs.Select(x => Path.GetFileName(x)).Select(x=> new Version(x));
            var needVersion = new Version("8.0.0");
            if (versions.Any(x => x >= needVersion)) return true;

            return false;
        }
    }
}
