import { httpClient } from 'src/api//base/httpClient'
import { IRequestPagination } from 'src/compositions/types'

export interface IProxy {
  id?: number
  name: string
  priority?: number
  emailMatch?: string
  description?: string
  isActive: boolean
  proxy: string
}

/**
 * 验证代理名称是否满足要求
 * @param name
 * @returns
 */
export function validateProxyName (name: string) {
  return httpClient.get<IProxy>('/proxy/valid-name', {
    params: {
      name
    }
  })
}

/**
 * 创建代理
 * @param proxyInfo
 * @returns
 */
export function createProxy (proxyInfo: IProxy) {
  return httpClient.post<IProxy>('/proxy', {
    data: proxyInfo
  })
}

/**
 * 获取代理数量
 * @param filter
 * @returns
 */
export function getProxiesCount (filter?: string) {
  return httpClient.get<number>('/proxy/filtered-count', {
    params: {
      filter
    }
  })
}

/**
 * 获取代理数据
 * @param filter
 * @param pagination
 * @returns
 */
export function getProxiesData (filter: string | undefined, pagination: IRequestPagination) {
  return httpClient.post<IProxy[]>('/proxy/filtered-count', {
    params: {
      filter
    },
    data: {
      pagination
    }
  })
}
