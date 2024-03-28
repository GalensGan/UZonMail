/* eslint-disable @typescript-eslint/no-explicit-any */
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios'
import axios from 'axios'

import { IHttpClientOptions, IResponseData } from './types'
import { useUserInfoStore } from 'src/stores/user'
import { StatusCode } from 'status-code-enum'
import { notifyError } from 'src/utils/notify'

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
    this.setRequestInterceptors(this._axios)
    this.setResponseInterceptors(this._axios)
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

  // 添加请求拦截器
  private setRequestInterceptors (axiosInstance: AxiosInstance) {
    axiosInstance.interceptors.request.use((config) => {
      const store = useUserInfoStore()
      // 自动添加 token
      config.headers.Authorization = 'Bearer ' + store.token
      return config
    },
    (error) => {
      return Promise.reject(error)
    })
  }

  // 添加响应拦截器
  private setResponseInterceptors (axiosInstance: AxiosInstance) {
    axiosInstance.interceptors.response.use(async (response) => {
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      const data = response.data as IResponseData<any>
      if (data.code !== StatusCode.SuccessOK) {
        // 处理错误
        if (this._options.notifyError) {
          // 提示错误
          notifyError(data.message)
        }

        // 返回错误
        return Promise.reject(data)
      }
      // 其它非 200 状态码
      return response
    },
    // 当 response.status 不是 200 时触发
    async (error) => {
      const response = error.response as AxiosResponse
      if (response.status === StatusCode.ClientErrorUnauthorized) {
        // 退出登陆
        await this.logout()
      }

      if (!response.data) {
        // 其它错误，进行提示，后端返回的错误，都会进行消息展示
        notifyError(response.statusText)
      } else {
        notifyError(response.data.message)
      }

      return Promise.reject(error)
    })
  }

  // 退出登陆
  private async logout () {

  }

  // #region 重写 axios 的 get、post、put、delete 方法

  /**
   * 通用请求
   * @param config
   * @returns
   */
  async request<R, C = any> (config: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    const result = await this._axios.request<C, IResponseData<R>>(config)
    return result
  }

  /**
   * get 请求
   * @param url 地址，不包含 baseUrl
   * @param config 请求参数和配置
   * @returns
   */
  async get<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.get<C, IResponseData<R>>(url, config)
  }

  /**
   * delete 请求
   * @param url
   * @param config
   * @returns
   */
  async delete<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.delete<C, IResponseData<R>>(url, config)
  }

  /**
   * head 请求
   * @param url
   * @param config
   * @returns
   */
  async head<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.head<C, IResponseData<R>>(url, config)
  }

  /**
   * options 请求
   * @param url
   * @param config
   * @returns
   */
  async options<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.options<C, IResponseData<R>>(url, config)
  }

  /**
   * post 请求
   * @param url
   * @param config
   * @returns
   */
  async post<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.post<C, IResponseData<R>>(url, config)
  }

  /**
   * put 请求
   * @param url
   * @param config
   * @returns
   */
  async put<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.put<C, IResponseData<R>>(url, config)
  }

  /**
   * patch 请求
   * @param url
   * @param config
   * @returns
   */
  async patch<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.patch<C, IResponseData<R>>(url, config)
  }

  async postForm<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.postForm<C, IResponseData<R>>(url, config)
  }

  async patchForm<R, C = any> (url: string, config?: AxiosRequestConfig<C>): Promise<IResponseData<R>> {
    return await this._axios.patchForm<C, IResponseData<R>>(url, config)
  }
  // #endregion
}

export const httpClient = new HttpClient({
  notifyError: true
})
