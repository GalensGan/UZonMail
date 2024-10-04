/* eslint-disable @typescript-eslint/no-explicit-any */
import { httpClientPro } from 'src/api/base/httpClient'

export enum UnsubscibeType {
  /// <summary>
  /// 系统
  /// </summary>
  System,

  /// <summary>
  /// 外部链接
  /// </summary>
  External
}

export interface IUnsubscribeSettings {
  id: number,

  /// <summary>
  /// 是否启用退订
  /// </summary>
  enable: boolean,

  /// <summary>
  /// 类型
  /// </summary>
  type: UnsubscibeType,

  /// <summary>
  /// 外部 URL
  /// </summary>
  externalUrl?: string,

  /// <summary>
  /// 退订按钮的 Id
  /// </summary>
  unsubscribeButtonId?: string,
}

/**
 * 退订
 * @param token
 * @returns
 */
export function unsubscribe (token: string) {
  return httpClientPro.post<boolean>('/unsubscribe', {
    params: {
      token
    }
  })
}

/**
 * 是否取消退订
 * @param token
 * @returns
 */
export function isUnsubscribed (token: string) {
  return httpClientPro.get<boolean>('/unsubscribe/unsubscribed', {
    params: {
      token
    }
  })
}

/**
 * 获取退定设置
 * @returns
 */
export function getUnsubscribeSettings () {
  return httpClientPro.get<IUnsubscribeSettings>('/unsubscribe')
}

/**
 * 更新退订设置
 * @param settingId
 * @param data
 * @returns
 */
export function updateUnsubscribeSettings (settingId: number, data: IUnsubscribeSettings) {
  return httpClientPro.put<boolean>(`/unsubscribe/${settingId}`, { data })
}
