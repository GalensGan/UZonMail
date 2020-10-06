using log4net;
using NPOI.OpenXml4Net.OPC.Internal;
using SendMultipleEmails.Datas;
using SendMultipleEmails.Enums;
using Stylet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Screen = Stylet.Screen;

namespace SendMultipleEmails.Pages
{
    public class SendViewModel : Conductor<Screen>.Collection.OneActive
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendViewModel));

        private Store _store;

        private IWindowManager _windowManager;

        #region 属性
        public Visibility IsShowNew { get; set; } = Visibility.Visible;

        public Visibility IsShowView { get; set; } = Visibility.Collapsed;
        #endregion
        public SendViewModel(Store store, IWindowManager windowManager)
        {
            _store = store;
            _windowManager = windowManager;
        }

        protected override void OnInitialActivate()
        {
            // 初始化
            SendScreenBase send = new Send_NewViewModel(_store)
            {
                DisplayName = SendStatus.New.ToString(),
            };
            send.CommandChanged += ActiveItemByStatus;
            this.Items.Add(send);

            send = new Send_PreviewViewModel(_store)
            {
                DisplayName = SendStatus.Preview.ToString(),
            };
            send.CommandChanged += ActiveItemByStatus;
            this.Items.Add(send);

            send = new Send_SendingViewModel(_store)
            {
                DisplayName = SendStatus.Sending.ToString(),
            };
            send.CommandChanged += ActiveItemByStatus;
            this.Items.Add(send);

            send = new Send_SentViewModel(_store)
            {
                DisplayName = SendStatus.Sent.ToString(),
            };
            send.CommandChanged += ActiveItemByStatus;
            this.Items.Add(send);

            ActiveItemByStatus(SendStatus.New);

            base.OnInitialActivate();
        }


        private void ActiveItemByStatus(SendStatus sendStatus)
        {
            Screen screen = this.Items.Where(item => item.DisplayName == sendStatus.ToString()).FirstOrDefault();
            if (screen is SendScreenBase scb)
            {
                this.ActivateItem(scb);
                scb.Load();
            }
        }
    }
}
