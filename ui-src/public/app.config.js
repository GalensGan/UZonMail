/* eslint-disable @typescript-eslint/no-explicit-any */
/**
 * 可以在这里配置一些全局的配置
 */

const prodConfig = {
  baseUrl: 'http://localhost:22345',
  api: '/api/v1',
  signalRHub: '/hubs/uzonMailHub',
  // 日志配置
  logger: {
    level: 'info'
  }
}

window.uzonMailConfig = prodConfig
