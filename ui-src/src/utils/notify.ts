import { Notify, QNotifyCreateOptions } from 'quasar'

/**
 * 错误
 * @param opts
 */
export function notifyError (opts: QNotifyCreateOptions | string): void {
  let inputOptions = opts
  if (typeof opts === 'string') { inputOptions = { message: opts } }

  const appNotifyOptions: QNotifyCreateOptions = {
    color: 'negative',
    icon: 'close',
    position: 'top'
  }

  const fullOptions = Object.assign(appNotifyOptions, inputOptions)
  Notify.create(fullOptions)
}

/**
 * 通知成功
 * @param opts
 */
export function notifySuccess (opts: QNotifyCreateOptions | string): void {
  let inputOptions = opts
  if (typeof opts === 'string') { inputOptions = { message: opts } }

  const appNotifyOptions: QNotifyCreateOptions = {
    color: 'positive',
    icon: 'done',
    position: 'top'
  }

  const fullOptions = Object.assign(appNotifyOptions, inputOptions)
  Notify.create(fullOptions)
}
