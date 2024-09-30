/* eslint-disable @typescript-eslint/no-explicit-any */
import { httpClientPro } from 'src/api/base/httpClient'

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
