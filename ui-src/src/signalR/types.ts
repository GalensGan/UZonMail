/**
 * 服务器可调用的方法
 * 当注册后，服务器即可调用该方法
 */
export enum UzonMailClientMethods {
  notify,

  userSendingProgressChanged,
  sendingGroupProgressChanged,
  sendingItemProgressChanged,
  groupStartSending,
  groupEndSending,

  sendError,
}

export interface IUserSendingProgressArg {
  startDate: string,
  total: number,
  current: number,
  message?: string
}

export interface ISendingGroupProgressArg {
  startDate: string,
  sendingGroupId: number,
  total: number,
  current: number,
  message?: string
}

/**
 * 开始发件参数
 */
export interface IGroupStartSendingArg {
  startDate: string,
  sendingGroupId: number,
  subjects: string,
  total: number,
  current: number,
  message?: string
}

/**
 * 结束发件
 */
export interface IGroupEndSendingArg {
  startDate: string,
  sendingGroupId: number,
  total: number,
  success: number,
  message?: string
}
