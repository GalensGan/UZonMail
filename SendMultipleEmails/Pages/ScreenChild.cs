using GalensSDK.StyletEx;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Pages
{
    public abstract class ScreenChild:Screen, IInvoke
    {
        public ScreenChild(Store store)
        {
            Store = store;
        }
        public Store Store { get;private set; }
        /// <summary>
        /// 图标名称
        /// </summary>
        public string IconName { get; set; }

        #region 接口实现
        public string ID { get; set; }

        public event Action<InvokeParameter> InvokeEvent;

        public virtual void AfterInvoke(InvokeParameter parameter)
        {
            
        }

        public virtual void BeforeInvoke(InvokeParameter parameter)
        {
            
        }

        /// <summary>
        /// 重置参数
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Reset(InvokeParameter parameter)
        {
            
        }

        /// <summary>
        /// 唤醒其它项
        /// </summary>
        /// <param name="parameter"></param>
        protected void InvokeTo(InvokeParameter parameter)
        {
            this.InvokeEvent?.Invoke(parameter);
        }
        #endregion
    }
}
