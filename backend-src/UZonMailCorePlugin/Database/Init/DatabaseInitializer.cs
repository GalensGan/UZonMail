﻿using UZonMail.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using UZonMail.Core.Utils.Database;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Organization;
using UZonMail.DB.SQL.Files;
using UZonMail.DB.SQL.EmailSending;
using UZonMail.Core.Config;
using UZonMail.DB.SQL.Settings;

namespace UZonMail.Core.Database.Init
{
    /// <summary>
    /// 初始化数据库
    /// 每次启动时，都需要执行
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <param name="sqlContext"></param>
    /// <param name="config"></param>
    public class DatabaseInitializer(IWebHostEnvironment hostEnvironment, SqlContext sqlContext, AppConfig config)
    {
        private readonly SqlContext _db = sqlContext;
        private IWebHostEnvironment _hostEnvironment = hostEnvironment;
        private AppConfig _appConfig = config;

        /// <summary>
        /// 开始执行初始化
        /// </summary>
        public async Task Init()
        {
            await ResetSendingItemsStatus();
            await ResetSendingGroup();
        }

        private async Task ResetSendingItemsStatus()
        {
            // 对所有的 Pending 状态的发件项重置为 Created
            await _db.SendingItems.UpdateAsync(x => x.Status == SendingItemStatus.Pending || x.Status == SendingItemStatus.Sending, 
                x => x.SetProperty(y => y.Status, SendingItemStatus.Created));
        }

        private async Task ResetSendingGroup()
        {
            // 将所有的 Sending 或者 Create 状态的发件组重置为 Finish
            await _db.SendingGroups.UpdateAsync(x => x.Status == SendingGroupStatus.Sending
                || (x.Status == SendingGroupStatus.Created && x.Status != SendingGroupStatus.Scheduled)
            , obj => obj.SetProperty(x => x.Status, SendingGroupStatus.Finish));
        }
    }
}
