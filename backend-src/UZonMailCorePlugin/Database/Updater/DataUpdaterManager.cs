using Microsoft.EntityFrameworkCore;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.Settings;
using UZonMail.Core.Database.Updater.Updaters;
using System.Reflection;
using UZonMail.Core.Config;

namespace UZonMail.Core.Database.Updater
{
    /// <summary>
    /// 数据升级管理器
    /// </summary>
    /// <param name="db"></param>
    public class DataUpdaterManager(SqlContext db, AppConfig config)
    {
        public readonly static Version RequiredVersion = new("1.0.2.0");
        private string _settingKey = "DataVersion";

        /// <summary>
        /// 获取所有的更新器
        /// 当新增更新器时，在此处添加
        /// </summary>
        /// <returns></returns>
        private List<IDataUpdater?> GetUpdaters()
        {
            // 从当前程序集中获取实现了 IDataUpdater 接口的类，并使用无参构造函数实例化
            var dataUpdaterType = typeof(IDataUpdater);
            var updaters = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && dataUpdaterType.IsAssignableFrom(x))
                .Select(x =>
                {
                    // 实例化
                    var instance = Activator.CreateInstance(x);
                    return instance as IDataUpdater;
                })
                .ToList();

            return updaters;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="config"></param>
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
            if (originVersion > RequiredVersion)
                throw new ArgumentException("数据库版本高于当前所需版本，请更新程序后再使用");

            // 若版本一致时，说明已经更新过
            if (originVersion == RequiredVersion) return;

            // 执行数据库升级
            // 实例化所有的更新类
            var updaters = GetUpdaters().Where(x => x != null).Where(x => x.Version > originVersion && x.Version <= RequiredVersion)
                .OrderBy(x => x.Version); // 升序排列
            foreach (var updater in updaters)
            {
                await updater.Update(db, config);
            }

            // 更新版本号
            versionSetting.StringValue = RequiredVersion.ToString();
            await db.SaveChangesAsync();
        }  
    }
}
