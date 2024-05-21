/**
 * 服务器可调用的方法
 * 当注册后，服务器即可调用该方法
 */
export enum UzonMailClientMethods {
  notify,

  sendingGroupTotalProgressChanged,
  SendingGroupProgressChanged,
  sendingItemProgressChanged,
  sendError,
}

export interface ISendingGroupProgressArg {
  startDate: string,
  sendingGroupId: number,
  total: number,
  current: number,
  message?: string
}
