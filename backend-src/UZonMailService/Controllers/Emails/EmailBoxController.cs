using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uamazing.ConfValidatation.Core.Entrance;
using Uamazing.ConfValidatation.Core.Validators;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.RequestModel;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SQL;
using UZonMailService.Models.SQL.Emails;
using UZonMailService.Models.SQL.UserInfos;
using UZonMailService.Services.Emails;
using UZonMailService.Services.Settings;
using UZonMailService.Services.UserInfos;
using UZonMailService.Utils.ASPNETCore.PagingQuery;
using UZonMailService.Utils.Database;
using UZonMailService.Utils.DotNETCore.Exceptions;

namespace UZonMailService.Controllers.Emails
{
    /// <summary>
    /// 邮箱
    /// </summary>
    public class EmailBoxController(SqlContext db, TokenService tokenService, UserService userService, EmailBoxService emailBoxService) : ControllerBaseV1
    {
        /// <summary>
        /// 创建发件箱
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("outbox")]
        public async Task<ResponseResult<Outbox>> CreateOutbox([FromBody] Outbox entity)
        {
            entity.Validate(new VdObj()
            {
                { ()=>entity.SmtpHost,new IsString("请输入 smtp 地址") },
                { ()=>entity.Email,new IsString("请输入邮箱"){ MinLength=3} },
                { ()=>entity.Password,new IsString("请输入密码")}
            }, ValidateOption.ThrowError);

            // 设置默认端口
            if (entity.SmtpPort == 0) entity.SmtpPort = 25; // 默认端口

            int userId = tokenService.GetIntUserId();
            // 验证发件箱是否存在，若存在，则复用原来的发件箱
            Outbox? existOne = db.Outboxes.SingleOrDefault(x => x.UserId == userId && x.Email == entity.Email && x.BoxType == EmailBoxType.Outbox);
            if (existOne != null)
            {
                existOne.EmailGroupId = entity.EmailGroupId;
                existOne.SmtpPort = entity.SmtpPort;
                existOne.Password = entity.Password;
                existOne.UserName = entity.UserName;
                existOne.Description = entity.Description;
                existOne.ProxyId = entity.ProxyId;
                existOne.SetStatusNormal();
            }
            else
            {
                // 新建一个发件箱
                entity.UserId = userId;
                db.Outboxes.Add(entity);
                existOne = entity;
            }
            await db.SaveChangesAsync();

            return existOne.ToSuccessResponse();
        }

        /// <summary>
        /// 批量新增发件箱
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("outboxes")]
        public async Task<ResponseResult<List<Outbox>>> CreateOutboxes([FromBody] List<Outbox> entities)
        {
            int userId = tokenService.GetIntUserId();
            foreach (var entity in entities)
            {
                entity.Validate(new VdObj()
                {
                    { ()=>entity.SmtpHost,new IsString("请输入 smtp 地址") },
                    { ()=>entity.Email,new IsString("请输入邮箱"){ MinLength=3} },
                    { ()=>entity.Password,new IsString("请输入密码")}
                }, ValidateOption.ThrowError);

                // 设置默认端口
                if (entity.SmtpPort == 0) entity.SmtpPort = 25; // 默认端口
                // 设置用户
                entity.UserId = userId;
            }
            List<string> emails = entities.Select(x => x.Email).ToList();
            List<Outbox> existEmails = await db.Outboxes.Where(x => x.UserId == userId && x.BoxType == EmailBoxType.Outbox && emails.Contains(x.Email)).ToListAsync();
            List<Outbox?> newEntities = emails.Except(existEmails.Select(x => x.Email))
                .Select(x => entities.Find(e => e.Email == x))
                .ToList();

            // 新建发件箱
            foreach (var entity in newEntities)
            {
                if (entity != null)
                    db.Outboxes.Add(entity);
            }

            // 更新现有的发件箱
            foreach (var entity in existEmails)
            {
                var newEntity = entities.Find(x => x.Email == entity.Email);
                if (newEntity != null)
                {
                    entity.EmailGroupId = newEntity.EmailGroupId;
                    entity.SmtpPort = newEntity.SmtpPort;
                    entity.UserName = newEntity.UserName;
                    entity.Password = newEntity.Password;
                    entity.EnableSSL = newEntity.EnableSSL;
                    entity.Description = newEntity.Description;
                    entity.ProxyId = newEntity.ProxyId;
                    entity.Name = newEntity.Name;
                    entity.SetStatusNormal();
                }
            }
            await db.SaveChangesAsync();

            // 返回所有的结果
            List<Outbox> results = [.. existEmails, .. newEntities];
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 创建发件箱
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("inbox")]
        public async Task<ResponseResult<Inbox>> CreateInbox([FromBody] Inbox entity)
        {
            if (string.IsNullOrEmpty(entity.Email)) throw new KnownException("请输入邮箱地址");
            int userId = tokenService.GetIntUserId();
            entity.UserId = userId;

            // 验证发件箱是否存在，若存在，则复用原来的发件箱
            Inbox? existOne = db.Inboxes.SingleOrDefault(x => x.UserId == userId && x.Email == entity.Email && x.BoxType == EmailBoxType.Inbox);
            if (existOne != null)
            {
                existOne.EmailGroupId = entity.EmailGroupId;
                existOne.Name = entity.Name;
                existOne.Description = entity.Description;
                existOne.SetStatusNormal();
            }
            else
            {
                // 新建一个发件箱               
                db.Inboxes.Add(entity);
                existOne = entity;
            }
            await db.SaveChangesAsync();

            return existOne.ToSuccessResponse();
        }

        /// <summary>
        /// 批量新增发件箱
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("inboxes")]
        public async Task<ResponseResult<List<Inbox>>> CreateInboxes([FromBody] List<Inbox> entities)
        {
            int userId = tokenService.GetIntUserId();
            foreach (var entity in entities)
            {
                if (string.IsNullOrEmpty(entity.Email)) throw new KnownException("请输入邮箱地址");
                // 设置用户
                entity.UserId = userId;
            }
            List<string> emails = entities.Select(x => x.Email).ToList();
            List<Inbox> existEmails = await db.Inboxes.Where(x => x.UserId == userId && x.BoxType == EmailBoxType.Inbox && emails.Contains(x.Email)).ToListAsync();
            List<Inbox?> newEntities = emails.Except(existEmails.Select(x => x.Email))
                .Select(x => entities.Find(e => e.Email == x))
                .ToList();

            // 新建发件箱
            foreach (var entity in newEntities)
            {
                if (entity != null)
                    db.Inboxes.Add(entity);
            }

            // 更新现有的发件箱
            foreach (var entity in existEmails)
            {
                var newEntity = entities.Find(x => x.Email == entity.Email);
                if (newEntity != null)
                {
                    entity.EmailGroupId = newEntity.EmailGroupId;
                    entity.Name = newEntity.Name;
                    entity.Description = newEntity.Description;
                    entity.SetStatusNormal();
                }
            }
            await db.SaveChangesAsync();

            // 返回所有的结果
            List<Inbox> results = [.. existEmails, .. newEntities];
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 更新发件箱
        /// </summary>
        /// <param name="outboxId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("outbox/{outboxId:int}")]
        public async Task<ResponseResult<bool>> UpdateOutbox(int outboxId, [FromBody] Outbox entity)
        {
            await db.Outboxes.UpdateAsync(x => x.Id == outboxId,
                 x => x.SetProperty(y => y.Email, entity.Email)
                 .SetProperty(y => y.Name, entity.Name)
                 .SetProperty(y => y.SmtpHost, entity.SmtpHost)
                 .SetProperty(y => y.SmtpPort, entity.SmtpPort)
                 .SetProperty(y=>y.UserName,entity.UserName)
                 .SetProperty(y => y.Password, entity.Password)
                 .SetProperty(y => y.EnableSSL, entity.EnableSSL)
                 .SetProperty(y => y.Description, entity.Description)
                 .SetProperty(y => y.ProxyId, entity.ProxyId)
                 );
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 更新发件箱
        /// </summary>
        /// <param name="inboxId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("inbox/{inboxId:int}")]
        public async Task<ResponseResult<bool>> UpdateInbox(int inboxId, [FromBody] Inbox entity)
        {
            await db.Inboxes.UpdateAsync(x => x.Id == inboxId,
                 x => x.SetProperty(y => y.Email, entity.Email)
                    .SetProperty(y=>y.Name,entity.Name)
                    .SetProperty(y => y.Description, entity.Description)
                 );
            return true.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮箱数量
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="emailBoxType"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("filtered-count")]
        public async Task<ResponseResult<int>> GetBoxesCount(int groupId, EmailBoxType emailBoxType, string filter)
        {
            int userId = tokenService.GetIntUserId();
            // 收件箱
            var dbSet = db.Inboxes.Where(x => x.BoxType == emailBoxType && x.UserId == userId && !x.IsDeleted && !x.IsHidden);
            if (groupId > 0)
            {
                dbSet = dbSet.Where(x => x.EmailGroupId == groupId);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Email.Contains(filter) || x.Description.Contains(filter));
            }
            int count = await dbSet.CountAsync();
            return count.ToSuccessResponse();
        }

        /// <summary>
        /// 获取邮箱数据
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="emailBoxType"></param>
        /// <param name="filter"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        [HttpPost("filtered-data")]
        public async Task<ResponseResult<List<Inbox>>> GetBoxesData(int groupId, EmailBoxType emailBoxType, string filter, [FromBody] Pagination pagination)
        {
            int userId = tokenService.GetIntUserId();
            var dbSet = db.Inboxes.Where(x => x.BoxType == emailBoxType && x.UserId == userId && !x.IsDeleted && !x.IsHidden);
            if (groupId > 0)
            {
                dbSet = dbSet.Where(x => x.EmailGroupId == groupId);
            }
            if (!string.IsNullOrEmpty(filter))
            {
                dbSet = dbSet.Where(x => x.Email.Contains(filter) || x.Description.Contains(filter));
            }
            var results = await dbSet.Page(pagination).ToListAsync();
            return results.ToSuccessResponse();
        }

        /// <summary>
        /// 通过 id 删除邮箱
        /// 若邮箱在使用，则仅标记一个删除状态
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpDelete("{emailBoxId:int}")]
        public async Task<ResponseResult<bool>> DeleteEmailBoxById(int emailBoxId)
        {
            var emailBox = await db.Inboxes.FirstOrDefaultAsync(x => x.Id == emailBoxId);
            if (emailBox == null) throw new KnownException("邮箱不存在");
            await emailBoxService.DeleteEmailBox(emailBox);

            return true.ToSuccessResponse();
        }
    }
}
