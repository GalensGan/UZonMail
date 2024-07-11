﻿using System.Net;
using Uamazing.Utils.Results;

namespace Uamazing.Utils.Web.ResponseModel
{
    /// <summary>
    /// 用于在控制器中方便返回结果值
    /// </summary>
    public class ResponseResult<T> : Result<T>
    {
        /// <summary>
        /// 错误代码，前端可以根据这些代码作不同的操作
        /// 不认成功或失败，只要没有特殊需求，它都为200
        /// </summary>
        public int Code { get; set; } = (int)HttpStatusCode.OK;

        public static ResponseResult<T> Success(T data) => new ResponseResult<T>() { Ok = false, Message = "ok", Data = data };
        public static ResponseResult<T> Fail(string message) => new ResponseResult<T>() { Ok = false, Message = message,Code = (int)HttpStatusCode.BadRequest };
    }
}
