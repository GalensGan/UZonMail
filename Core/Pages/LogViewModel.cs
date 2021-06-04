using log4net.Core;
using SendMultipleEmails.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SendMultipleEmails.Pages
{
    class LogViewModel : ScreenChild
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LogViewModel));
        private bool _logWatching = true;
        private log4net.Appender.MemoryAppender logger;
        private Thread logWatcher;

        public LogViewModel(Store store) : base(store) { }
        public string Logs { get; set; }
        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            // 第一加载的时候，加载信息
            logger = new log4net.Appender.MemoryAppender();
            log4net.Config.BasicConfigurator.Configure(logger);
            logWatcher = new Thread(new ThreadStart(LogWatcher));
            logWatcher.IsBackground = true;
            logWatcher.Start();

        }

        public Visibility NoneLog { get; set; } = Visibility.Visible;

        public Visibility ShowLog { get; set; } = Visibility.Collapsed;

        private void LogWatcher()
        {
            while (_logWatching)
            {
                LoggingEvent[] events = logger.GetEvents();
                if (events != null && events.Length > 0)
                {
                    logger.Clear();
                    foreach (LoggingEvent ev in events)
                    {
                        string line = ev.LoggerName + ": " + ev.RenderedMessage + "\r\n";
                        AppendLog(line);
                    }
                }
                Thread.Sleep(500);
            }
        }

        void AppendLog(string _log)
        {
            if(NoneLog==Visibility.Visible && _log.Length > 0)
            {
                NoneLog = Visibility.Collapsed;
                ShowLog = Visibility.Visible;
            }
            StringBuilder builder = new StringBuilder(Logs);
            builder.Append(_log);
            Logs = builder.ToString();
        }
    }
}
