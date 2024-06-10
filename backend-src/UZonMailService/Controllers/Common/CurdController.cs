using Microsoft.AspNetCore.Mvc;
using Uamazing.Utils.Web.Extensions;
using Uamazing.Utils.Web.ResponseModel;
using UZonMailService.Models.SqlLite.Base;
using UZonMailService.Services.Common;

namespace UZonMailService.Controllers.Common
{
    /// <summary>
    /// 通用的增删改查控制器
    /// </summary>
    public abstract class CurdController<T>(CurdService<T> curdService) : ControllerBaseV1 where T : SqlId
    {
        /// <summary>
        /// 增加数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<ResponseResult<T>> Create([FromBody] T entity)
        {
            // 需要判断是否重复
            var result = await curdService.Create(entity);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 更新
        /// 若要起作用，则子类需要重新实现该方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPut("{id:int}")]
        public abstract Task<ResponseResult<T>> Update(int id, [FromBody] T entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public virtual async Task<ResponseResult<bool>> Delete(int id)
        {
            var result = await curdService.DeleteById(id);
            return result.ToSuccessResponse();
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public virtual async Task<ResponseResult<T?>> FindOneById(int id)
        {
            var result = await curdService.FindOneById(id);
            return result.ToSuccessResponse();
        }
    }
}
