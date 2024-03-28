import { httpClient } from './base/httpClient'

/**
 * 测试 mock
 * @returns
 */
export async function apiGet () {
  return await httpClient.axios.get('/api/get')
}
