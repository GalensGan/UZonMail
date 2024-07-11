/* eslint-disable @typescript-eslint/no-explicit-any */
import _ from 'lodash'
import appConfig from 'app/public/app.config'
import { QuasarContext } from '@quasar/app-vite/types/configuration/context'

let config: Record<string, any> | null = null
export function useConfig (quasarContext?: QuasarContext) {
  if (config) return config

  // 参考：https://vitejs.dev/guide/env-and-mode.html
  // console.log(import.meta.env, '----', process.env)
  const isDev = quasarContext ? quasarContext.dev : import.meta.env.DEV

  // 解析 config
  const configTemp = _.merge(appConfig.default, isDev ? appConfig.dev : appConfig.prod) as Record<string, any>
  config = Object.assign({}, import.meta.env, configTemp)
  return config
}
