/* eslint-disable @typescript-eslint/no-explicit-any */
import { QuasarContext } from '@quasar/app-vite/types/configuration/context'
import appConfigs from './app.config'
import { IAppConfig } from './types'

/**
 * 具体的配置接口
 */

let config: IAppConfig = appConfigs.default
export function useConfig (): IAppConfig {
  return config
}

/**
 * 使用配置
 * @param quasarContext
 * @returns
 */
export async function useConfigAsync (quasarContext: QuasarContext): Promise<IAppConfig> {
  // 参考：https://vitejs.dev/guide/env-and-mode.html
  // console.log(import.meta.env, '----', process.env)
  const isDev = quasarContext ? quasarContext.dev : import.meta.env.DEV
  // 解析 config
  const configTemp = Object.assign({}, appConfigs.default, isDev ? appConfigs.dev : appConfigs.prod)
  config = Object.assign({}, import.meta.env, configTemp)
  console.log('[config] ', '当前配置：', config)
  return config
}

/**
 * 修改配置
 * @param newConfig
 */
export function changeConfig (newConfig: IAppConfig) {
  console.log('[config] 修改配置:', newConfig)
  Object.assign(config, newConfig)
}
