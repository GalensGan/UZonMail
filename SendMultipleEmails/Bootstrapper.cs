using System;
using Stylet;
using StyletIoC;
using SendMultipleEmails.Pages;
using log4net.Core;
using log4net;

namespace SendMultipleEmails
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Bootstrapper));
        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // Configure the IoC container in here
        }

        protected override void Configure()
        {
            // Perform any other configuration before the application starts
        }

        protected override void OnStart()
        {
            Stylet.Logging.LogManager.Enabled = true;

            // 添加对所有未捕获异常的读取
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error("未捕获异常:" + e.ExceptionObject.ToString());
        }
    }
}
