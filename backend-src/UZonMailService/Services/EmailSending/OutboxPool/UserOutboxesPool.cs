using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Uamazing.Utils.Results;
using Uamazing.Utils.Web.Service;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.MultiTenant;
using UZonMailService.Services.EmailSending.Base;
using UZonMailService.Services.EmailSending.Event;
using UZonMailService.Services.EmailSending.Event.Commands;
using UZonMailService.Services.EmailSending.Models;

namespace UZonMailService.Services.EmailSending.OutboxPool
{
    /// <summary>
    /// 单个用户的发件箱池
    /// 每个邮箱账号共用冷却池
    /// key: 邮箱 userId+邮箱号 ，value: 发件箱列表
    /// </summary>
    public class UserOutboxesPool : DictionaryManager<OutboxEmailAddress>, IDictionaryItem
    {
        private readonly IServiceScopeFactory _ssf;
        public UserOutboxesPool(IServiceScopeFactory ssf, long userId)
        {
            _ssf = ssf;
            UserId = userId;
            Key = userId.ToString();

            // 添加事件监听
            EventCenter.Core.DataChanged += EventCenterDataChanged;
        }

        #region 事件处理与触发
        /// <summary>
        /// 监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async Task EventCenterDataChanged(object? sender, CommandBase e)
        {
            switch (e.CommandType)
            {
                case CommandType.DisposeUserOutboxPoolCommand:
                    await OnDisposeUserOutboxPoolCommand(e);
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// 当发件任务被释放时
        /// 移除所有的发件箱
        /// </summary>
        private async Task OnDisposeUserOutboxPoolCommand(CommandBase e)
        {
            // 转换成发件任务结束的参数
            if (e is not DisposeUserOutboxPoolCommand args) return;
            if (args.Data != UserId) return;

            // 移除用户的发件池
            TryRemove(args.Data.ToString(), out _);
        }
        #endregion

        #region 自定义参数
        public long UserId { get; }
        #endregion

        #region 接口实现
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 字典键
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// 最小比例
        /// </summary>
        public double MinPercent { get; set; }
        /// <summary>
        /// 最大比例
        /// </summary>
        public double MaxPercent { get; set; }
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Enable { get; private set; }
        #endregion

        /// <summary>
        /// 添加发件箱
        /// </summary>
        /// <param name="outbox"></param>
        public async Task<bool> AddOutbox(OutboxEmailAddress outbox)
        {
            if (this.TryGetValue(outbox.Key, out var existValue))
            {
                existValue.Update(outbox);
                return true;
            }

            // 验证发件箱是否有效
            if (!outbox.Validate())
            {
                return false;
            }


            // 不存在则添加
            if (this.TryAdd(outbox.Key, outbox))
            {
                Enable = true;
            }
            return true;
        }

        /// <summary>
        /// 移除当前用户下发件组对应的发件箱
        /// 移除后，发件箱不一定为空
        /// </summary>
        /// <param name="groupId"></param>
        private async Task RemoveOutbox2(long groupId)
        {
            // 找到包含该组的发件箱
            var outboxes = this.Values.Where(x => x.SendingGroupIds.Contains(groupId));
            foreach (var outbox in outboxes)
            {
                outbox.SendingGroupIds.Remove(groupId);
                if (outbox.SendingGroupIds.Count != 0) continue;

                // 释放 outbox
                if (this.TryRemove(outbox.Key, out _))
                {
                    // 触发移除事件                   
                    outbox.Dispose();
                }
            }
        }

        /// <summary>
        /// 移除用户某个发件箱
        /// </summary>
        /// <param name="outboxEmail"></param>
        public async Task RemoveOutbox2(string outboxEmail)
        {
            // 获取发件箱列表
            var values = Values.Where(x => x.Email == outboxEmail);
            // 开始移除
            foreach (var outbox in values)
            {
                if (this.TryRemove(outbox.Key, out _))
                {
                    // 触发移除事件
                    outbox.Dispose();
                }
            }
        }

        /// <summary>
        /// 按用户设置的权重获取发件箱
        /// </summary>
        /// <returns></returns>
        public async Task<FuncResult<OutboxEmailAddress>> GetOutboxByWeight(ScopeServices scopeServices)
        {
            var data = GetDataByWeight();
            if (data.NotOk) return data;

            // 若有 outbox
            var outbox = data.Data;
            // 取出来后，进入冷却时间
            // 避免被其它线程取出
            var occupySuccess = await outbox.SetCooldown(scopeServices);
            if(!occupySuccess)
            {
                return new FuncResult<OutboxEmailAddress>()
                {
                    Ok = false,
                    Status = PoolResultStatus.CooldownError,
                    Message = "发件箱冷却中"
                };
            }

            return new FuncResult<OutboxEmailAddress>()
            {
                Data = outbox,
                Ok = true
            };
        }

        public void Dispose()
        {
        }
    }
}
