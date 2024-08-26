/* eslint-disable @typescript-eslint/no-explicit-any */
import _ from 'lodash'
import { QuasarContext } from '@quasar/app-vite/types/configuration/context'
import appConfigs from './app.config'
import { IAppConfig } from './types'
import logger from 'loglevel'

/**
 * 具体的配置接口
 */

let config: IAppConfig = appConfigs.default
export function useConfig (): IAppConfig {
  return config
}

export async function useConfigAsync (quasarContext: QuasarContext): Promise<IAppConfig> {
  logger.debug('[config] ', '生成配置')
  // 从服务器获取 /app.config.ts 获取配置
  const origin = typeof window !== 'undefined' ? window.location.origin : ''
  const fetchConfig = new Promise((resolve) => {
    if (!origin) resolve(appConfigs)
    const configUrl = `${origin}/app.config.js`
    logger.debug('[config] ', `正在从${configUrl}获取配置`)
    fetch(configUrl)
      .then(response => response.json())
      .then(config => {
        resolve(config)
      })
      .catch(() => {
        resolve(appConfigs)
      })
  })
  const appConfig = (await fetchConfig) as IAppConfig

  // 参考：https://vitejs.dev/guide/env-and-mode.html
  // console.log(import.meta.env, '----', process.env)
  const isDev = quasarContext ? quasarContext.dev : import.meta.env.DEV
  // 解析 config
  const configTemp = _.merge(appConfig, isDev ? appConfigs.dev : {})
  config = Object.assign({}, import.meta.env, configTemp)
  return config
}
