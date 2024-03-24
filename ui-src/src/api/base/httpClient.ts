import type { AxiosInstance } from 'axios'
import axios from 'axios'

import { IHttpClientOptions } from './types'

/**
 * HttpClient 封装
 */
export default class HttpClient {
  private _options: IHttpClientOptions
  private _axios: AxiosInstance

  public get axios () {
    return this._axios
  }

  constructor (options: IHttpClientOptions) {
    this._options = options
    this._axios = this.createAxios()
    this.setRequestInterceptors()
    this.setResponseInterceptors()
  }

  // 获取基础 url
  private getBaseUrl (): string {
    return process.env.BASE_URL as string
  }

  // 创建 axios 实例
  private createAxios () {
    return axios.create({
      baseURL: this.getBaseUrl(),
      responseType: 'json'
    })
  }

  private setRequestInterceptors () {
    axios.interceptors.request.use((config) => {
      // 验证 token 是有效期，忽略某些页面

      // 若 token 过期，则退出登陆

      // 自动添加 token
      if (config.headers) {
        // config.headers
      }

      return config
    },
    (error) => {
      return Promise.reject(error)
    })
  }

  private setResponseInterceptors () {
    axios.interceptors.response.use((response) => {
      return response
    },
    (error) => {
      return Promise.reject(error)
    })
  }
}

export const httpClient = new HttpClient({})
