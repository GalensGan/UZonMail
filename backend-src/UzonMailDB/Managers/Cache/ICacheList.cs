using UZonMail.DB.SQL.Base;

namespace UZonMail.DB.Managers.Cache
{
    public interface ICacheList<T> : ICache where T : SqlId
    {
        void SetDirty
    }
}
