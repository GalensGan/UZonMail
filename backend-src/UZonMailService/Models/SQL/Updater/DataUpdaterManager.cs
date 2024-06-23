using Microsoft.EntityFrameworkCore;
using UZonMailService.Models.SQL.Settings;
using UZonMailService.Models.SQL.Updater.Updaters;

namespace UZonMailService.Models.SQL.Updater
{
    public class DataUpdaterManager(SqlContext db)
    {
        private readonly Version _currentVersion = new("0.7.0.0");
        private string _settingKey = "DataVersion";

        /// <summary>
        /// 获取所有的更新器
        /// 当新增更新器时，在此处添加
        /// </summary>
        /// <returns></returns>
        private List<IDataUpdater> GetUpdaters()
        {
            return
            [
                new UserDepartmentInfoUpdater()
            ];
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task Update()
        {
            // 获取数据版本
            var versionSetting = await db.SystemSettings.FirstOrDefaultAsync(x => x.Key == _settingKey);
            if (versionSetting == null)
            {
                // 初始化版本
                versionSetting = new SystemSetting
                {
                    Key = _settingKey,
                    StringValue = "0.0.0.0"
                };
                db.SystemSettings.Add(versionSetting);
            }

            var originVersion = new Version(versionSetting.StringValue);
            if (originVersion > _currentVersion)
                throw new ArgumentException("数据库版本高于当前所需版本，请更新程序后再使用");

            // 若版本一致时，说明已经更新过
            if (originVersion == _currentVersion) return;

            // 执行数据库升级
            // 实例化所有的更新类
            var updaters = GetUpdaters().Where(x => x.Version > originVersion && x.Version <= originVersion)
                .OrderBy(x => x.Version);

            // 将其排序
            foreach (var updater in updaters)
            {
                await updater.Update(db);
            }

            // 更新版本号
            versionSetting.StringValue = _currentVersion.ToString();
            await db.SaveChangesAsync();
        }
    }
}
