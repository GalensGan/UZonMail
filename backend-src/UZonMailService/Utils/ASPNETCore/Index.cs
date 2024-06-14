using LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using UZonMailService.Config.SubConfigs;
using UZonMailService.Utils.DotNETCore;
using UZonMailService.Utils.DotNETCore.Convention;
using Uamazing.Utils.Web.Service;
using UZonMailService.Utils.Database;
using Microsoft.Extensions.Options;

namespace UZonMailService.Utils.DotNETCore
{
    /// <summary>
    /// DotNETCore 扩展类
    /// </summary>
    internal static class Index
    {
        /// <summary>
        /// 设置 slugify-case 形式的路由
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection SetupSlugifyCaseRoute(this IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(
                                             new SlugifyParameterTransformer()));
            });
            return services;
        }

        /// <summary>
        /// 批量注入服务
        /// </summary>
        /// <typeparam name="T">通过指定类型,来注入所有实现该接口的单例。若要全部注册，只需传入 IService 即可</typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // 批量注入 Services 单例            
            var serviceTypes = Assembly.GetCallingAssembly()
                .GetTypes();
            var transientType = typeof(ITransientService);
            // 分多种情况，注册不同的生命周期
            var transientTypes = serviceTypes.Where(x => !x.IsAbstract && transientType.IsAssignableFrom(x))
                .ToList();
            transientTypes.ForEach(type => services.AddTransient(type));

            // 请求周期
            var scopedServiceType = typeof(IScopedService);
            // 分多种情况，注册不同的生命周期
            var scopedServiceTypes = serviceTypes.Where(x => !x.IsAbstract && scopedServiceType.IsAssignableFrom(x))
                .ToList();
            scopedServiceTypes.ForEach(type => services.AddScoped(type));

            // 单例
            var singletonServiceType = typeof(ISingletonService);
            // 分多种情况，注册不同的生命周期
            var singletonServiceTypes = serviceTypes.Where(x => !x.IsAbstract && singletonServiceType.IsAssignableFrom(x))
               .ToList();
            singletonServiceTypes.ForEach(type => services.AddSingleton(type));

            return services;
        }

        /// <summary>
        /// 配置 swagger
        /// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        /// </summary>
        /// <param name="services"></param>
        /// <param name="apiInfo"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services, OpenApiInfo apiInfo, string xmlCommentsPath)
        {
            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc("v1", apiInfo);

                // Set the comments path for the Swagger JSON and UI.    
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsPath);
                swaggerOptions.IncludeXmlComments(xmlPath);

                // Bearer 的scheme定义
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    //参数添加在头部
                    In = ParameterLocation.Header,
                    //使用Authorize头部
                    Type = SecuritySchemeType.Http,
                    //内容为以 bearer开头
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                //把所有方法配置为增加bearer头部信息
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                           Reference = new OpenApiReference
                           {
                               Type = ReferenceType.SecurityScheme,
                               Id = "bearerAuth"
                           }
                        },
                        Array.Empty<string>()
                    }
                };

                //注册到swagger中
                swaggerOptions.AddSecurityDefinition("bearerAuth", securityScheme);
                swaggerOptions.AddSecurityRequirement(securityRequirement);
            });

            return services;
        }

        /// <summary>
        /// 配置 jwt 验证
        /// 参考：https://learn.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0
        /// </summary>
        /// <param name="services"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        public static IServiceCollection AddJWTAuthentication(this IServiceCollection services, string secretKey)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // 是否验证令牌有效期
                    ValidateLifetime = true,
                    // 每次颁发令牌，令牌有效时间
                    ClockSkew = TimeSpan.FromMinutes(1440)
                };

                // We have to hook the OnMessageReceived event in order to
                // allow the JWT authentication handler to read the access
                // token from the query string when a WebSocket or 
                // Server-Sent Events request comes in.

                // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
                // due to a limitation in Browser APIs. We restrict it to only calls to the
                // SignalR hub in this code.
                // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
                // for more information about security considerations when using
                // the query string to transmit the access token.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }

        /// <summary>
        /// 注册 LiteDB 实例
        /// liteDB 数据库文件名在配置中用 Database:LiteDbPath 指定，相对于程序目录
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddLiteDB(this IServiceCollection services)
        {
            services.AddSingleton(typeof(ILiteRepository), typeof(LiteDBContext));
            return services;
        }
    }
}
