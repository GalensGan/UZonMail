import { IRequestPagination } from 'src/compositions/types'
import { httpClient } from './base/httpClient'

export interface ISendingGroup {
  id: number
  subject: string
  fromEmail: string,
  inboxes: object[],
  sendDate: string,
  status: number,
  SendResult?: string
}

/**
 * 获取模板数量
 * @param filter
 * @returns
 */
export function getSendingItemsCount (sendingGroupId: number, filter?: string) {
  return httpClient.get<number>('/sending-item/filtered-count', {
    params: {
      sendingGroupId,
      filter
    }
  })
}

/**
 * 获取模板数据
 * @param filter
 * @param pagination
 * @returns
 */
export function getSendingItemsData (sendingGroupId: number, filter: string | undefined, pagination: IRequestPagination) {
  return httpClient.post<ISendingGroup[]>('/sending-item/filtered-data', {
    params: {
      sendingGroupId,
      filter
    },
    data: pagination
  })
}
