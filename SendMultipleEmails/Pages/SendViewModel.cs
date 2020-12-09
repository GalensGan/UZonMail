using GalensSDK.StyletEx;
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
    public class SendViewModel : KeyOneActive<ScreenChild>
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SendViewModel));

        private Store _store;

        #region 属性
        public Visibility IsShowNew { get; set; } = Visibility.Visible;

        public Visibility IsShowView { get; set; } = Visibility.Collapsed;
        #endregion
        public SendViewModel(Store store) { _store = store; }
    }
}
