using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UZonMail.DB.SQL;
using UZonMail.DB.SQL.MultiTenant;

namespace Uamazing.Utils.Web.Token
{
    public class TokenClaimsBuilders
    {
        private static readonly List<ITokenClaimBuilder> _builders = [];


        public static void AddBuilder(ITokenClaimBuilder builder)
        {
            _builders.Add(builder);
        }

        public static async Task<List<Claim>> GetClaims(IServiceProvider serviceProvider, User userInfo)
        {
            List<Claim> claims = [];
            foreach (var builder in _builders)
            {
                var builderClaims = await builder.Build(serviceProvider, userInfo);
                claims.AddRange(builderClaims);
            }
            return claims;
        }
    }
}
