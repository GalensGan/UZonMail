using JWT;
using JWT.Algorithms;
using JWT.Exceptions;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Http.Helpers
{
    class JWTHelper
    {
        /// <summary>
        /// 创建token
        /// </summary>
        /// <returns></returns>
        public static string CreateJwtToken(IDictionary<string, object> payload, string secret)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret);
            return token;
        }


        /// <summary>
        /// 校验解析token
        /// </summary>
        /// <returns></returns>
        public static bool ValidateJwtToken(string token, string secret, out string result)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm alg = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, alg);
                result = decoder.Decode(token, secret, true);
                //校验通过，返回解密后的字符串
                return true;
            }
            catch (TokenExpiredException)
            {
                //表示过期
                result = "expired";
                return false;
            }
            catch (SignatureVerificationException)
            {
                //表示验证不通过
                result = "invalid";
                return false;
            }
            catch (Exception e)
            {
                result = e.Message;
                return false;
            }

        }
    }
}
