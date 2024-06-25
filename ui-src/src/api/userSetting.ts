/* eslint-disable @typescript-eslint/no-explicit-any */
import { httpClient } from 'src/api//base/httpClient'

export interface IUserSetting {
  userId: string,
  maxSendCountPerEmailDay: number,
  minOutboxCooldownSecond: number,
  maxOutboxCooldownSecond: number,
  maxSendingBatchSize: number,
  minInboxCooldownHours: number,
  replyToEmails?: string
}

/**
 * 获取用户设置
 * @returns
 */
export function getCurrentUserSetting () {
  return httpClient.get<IUserSetting>('/user-setting')
}

/**
 * 更新用户设置
 * @param userSetting
 * @returns
 */
export function updateUserSetting (userSetting: IUserSetting) {
  return httpClient.put<Record<string, any>>('/user-setting', {
    data: userSetting
  })
}
