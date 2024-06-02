/**
 * 服务器可调用的方法
 * 当注册后，服务器即可调用该方法
 */
export enum UzonMailClientMethods {
  notify,

  sendingGroupProgressChanged,
  sendingItemStatusChanged,

  sendError,
}

export enum SendingGroupProgressType {
  start,
  sending,
  end
}

export interface ISendingGroupProgressArg {
  sendingGroupId: number,
  startDate: string,
  total: number,
  current: number,
  successCount: number, // 成功的数量
  sentCount: number,
  subject: string,
  progressType: SendingGroupProgressType,
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

/**
 * 发送项进度变化
 */
export interface ISendingItemStatusChangedArg {
  sendingItemId: number,
  status: number,
  sendResult: string,
  triedCount: number,
  fromEmail: string,
  outboxes: object[],
  sendDate: string,
  subject: string
}
