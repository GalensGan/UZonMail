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
        public SendViewModel(Store store)
        {
            _store = store;
        }

        protected override void OnInitialActivate()
        {
            // 添加子界面
            #region 发送模块
            // 初始化
            RegisterItem(new Send_IndexViewModel(_store)
            {
                DisplayName = SendStatus.New.ToString(),
                ID = InvokeID.Send_Index.ToString(),
            });

            RegisterItem(new Send_NewViewModel(_store)
            {
                DisplayName = SendStatus.New.ToString(),
                ID = InvokeID.Send_New.ToString(),
            });

            InvokeTo(new InvokeParameter() { InvokeId = InvokeID.Send_Index.ToString() });
            #endregion

            base.OnInitialActivate();
        }
    }
}
