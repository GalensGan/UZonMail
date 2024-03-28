/**
 * 初始化化 Client 的参数
 */
export interface IHttpClientOptions {
  // 提示错误
  notifyError?: boolean
}

/**
 * 返回值参数
 */
export interface IResponseData<T> {
  data: T,
  code: number,
  message: string,
  ok: boolean,
}
