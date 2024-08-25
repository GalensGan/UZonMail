/* eslint-disable @typescript-eslint/no-explicit-any */
/**
 * 可以在这里配置一些全局的配置
 */

export default {
  // 默认配置
  default: {
    baseUrl: 'http://localhost:22345',
    api: '/api/v1',
    signalRHub: '/hubs/uzonMailHub',
    // 日志配置
    logger: {
      level: 'info'
    }
  },

  // 生产配置
  prod: {},

  // 开发配置
  dev: {
    baseUrl: 'http://localhost:22345',
    api: '/api/v1',
    signalRHub: '/hubs/uzonMailHub',
    // 日志配置
    logger: {
      level: 'debug'
    }
  }
}
