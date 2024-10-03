/* eslint-disable @typescript-eslint/no-explicit-any */
import { httpClient } from 'src/api//base/httpClient'

export interface IOrganizationSetting {
  userId: string,
  maxSendCountPerEmailDay: number,
  minOutboxCooldownSecond: number,
  maxOutboxCooldownSecond: number,
  maxSendingBatchSize: number,
  minInboxCooldownHours: number,
  replyToEmails?: string,
  enableEmailTracker: boolean | null,
}

/**
 * 获取用户设置
 * @returns
 */
export function getCurrentOrganizationSetting () {
  return httpClient.get<IOrganizationSetting>('/organization-setting')
}

/**
 * 更新用户设置
 * @param organizationSetting
 * @returns
 */
export function updateOrganizationSetting (organizationSetting: IOrganizationSetting) {
  return httpClient.put<Record<string, any>>('/organization-setting', {
    data: organizationSetting
  })
}
