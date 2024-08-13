import { boot } from 'quasar/wrappers'
import log, { LogLevelNames } from 'loglevel'

/**
 * 配置 logger
 */
export default boot(() => {
  // 设置日志级别
  const logLevel = (process.env.LOG_LEVEL || 'info') as LogLevelNames
  console.log('[logger] log level is set to', logLevel)
  log.setLevel(logLevel)

  if (window) {
    // 向 windows 暴露 setLogLevel 方法
    window.setLogLevel = (level: LogLevelNames) => {
      console.log('[logger] set log level to', level)
      log.setLevel(level)
    }
  }
})
