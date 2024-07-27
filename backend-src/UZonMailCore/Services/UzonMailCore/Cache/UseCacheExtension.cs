using UZonMail.DB.MySql;
using UZonMail.DB.SQL;
using UZonMail.DB.SqLite;

namespace UZonMail.Core.Cache
{
    public static class UseCacheExtension
    {
        /// <summary>
        /// 使用缓存服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection? AddCache(this IServiceCollection? services)
        {
            if (services == null) return null;
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>() ?? throw new ArgumentNullException(nameof(IConfiguration));
            var cacheService = new CacheService(configuration);
            services.AddSingleton(cacheService);
            return services;
        }
    }
}
