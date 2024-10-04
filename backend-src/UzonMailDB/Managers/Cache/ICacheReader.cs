using System.Threading.Tasks;
using UZonMail.DB.SQL;

namespace UZonMail.Managers.Cache
{
    /// <summary>
    /// 设置接口
    /// </summary>
    public interface ICacheReader
    {
        string SettingKey { get;}

        /// <summary>
        /// 初始化
        /// 每次调用时，都需要调用该方法
        /// </summary>
        /// <param name="db"></param>
        /// <param name="key"></param>
        Task Initialize(SqlContext db, string key);

        /// <summary>
        /// 设置需要更新
        /// </summary>
        void NeedUpdate();
    }
}
