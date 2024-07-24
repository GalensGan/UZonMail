using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Controllers.Common;
using UZonMailService.UzonMailDB.SQL.Emails;
using UZonMailService.Services.Common;
using UZonMailService.Services.Emails;
using UZonMailService.Services.Settings;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 邮件管理
    /// </summary>
    public class EmailGroupController(EmailGroupService groupService, TokenService tokenService) : CurdController<EmailGroup>(groupService)
    {
        /// <summary>
        /// 创建邮件组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<ResponseResult<EmailGroup>> Create([FromBody] EmailGroup entity)
        {
            var userId = tokenService.GetUserDataId();
            entity.UserId = userId;
           
            EmailGroup emailGroup = await groupService.Create(entity);
            return emailGroup.ToSuccessResponse();
        }

        /// <summary>
        /// 获取用户组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("all")]
        public async Task<ResponseResult<List<EmailGroup>>> GetEmailGroups([FromQuery] EmailGroupType type)
        {
            var userId = tokenService.GetUserDataId();
            var groups = await groupService.GetEmailGroups(userId, type);
            return groups.ToSuccessResponse();
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override async Task<ResponseResult<EmailGroup>> Update(long id, [FromBody] EmailGroup entity)
        {
            // 数据验证
            if (string.IsNullOrEmpty(entity.Name)) throw new KnownException("组名不允许为空");

            entity.Id = id;
            await groupService.Update(entity, [nameof(EmailGroup.Name), nameof(EmailGroup.Description), nameof(EmailGroup.Order)]);
            return entity.ToSuccessResponse();
        }

        /// <summary>
        /// 根据 id 删除组
        /// 组可能已经被使用，若被使用，则不允许删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override async Task<ResponseResult<bool>> Delete(long id)
        {
            var result = await groupService.DeleteById(id);
            return result.ToSuccessResponse();
        }
    }
}
