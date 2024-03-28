import { httpClient } from './base/httpClient'

import { IUserInfo } from 'src/stores/types'
export interface IUserLoginInfo {
  token: string,
  access: string[],
  userInfo: IUserInfo
}
/**
 * 测试 mock
 * @returns
 */
export async function userLogin () {
  return await httpClient.post<IUserLoginInfo>('/user/login')
}
