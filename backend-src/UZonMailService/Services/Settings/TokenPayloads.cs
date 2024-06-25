using Newtonsoft.Json.Linq;
using System.Security.Claims;
using UZonMailService.Models.SQL.MultiTenant;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 生成 token 时的 payload
    /// </summary>
    public class TokenPayloads : Dictionary<string, string>
    {
        public TokenPayloads(User? userInfo)
        {
            if (userInfo == null)
                return;

            Add("userId", userInfo.Id.ToString());
            Add("userName", userInfo.UserId);
            // 授予角色，接口权限控制
            Add(ClaimTypes.Role, userInfo.IsSuperAdmin ? "admin" : "user");
            // signalR 需要使用此处的 Name
            Add(ClaimTypes.Name, userInfo.Id.ToString());
            // 参考:https://learn.microsoft.com/zh-cn/aspnet/core/signalr/groups?view=aspnetcore-8.0
            Add(ClaimTypes.NameIdentifier, userInfo.Id.ToString());
            Add("organizationId", userInfo.OrganizationId.ToString());
            Add("departmentId", userInfo.DepartmentId.ToString());
        }

        public TokenPayloads(JObject? tokenObj)
        {
            if (tokenObj == null)
                return;

            foreach (var item in tokenObj)
            {
                Add(item.Key, item.Value?.ToString() ?? "");
            }
        }

        /// <summary>
        /// userName 等效于字符串型的 userId
        /// </summary>
        public string UserName => this["UserId"];
        public long UserId => long.Parse(this["userId"]);
        public long OrganizationId => long.Parse(this["organizationId"]);
        public long DepartmentId => long.Parse(this["departmentId"]);
    }
}
