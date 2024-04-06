import { Notify, QNotifyCreateOptions, Dialog } from 'quasar'

/**
 * 错误
 * @param opts
 */
export function notifyError (opts: QNotifyCreateOptions | string | undefined): void {
  if (!opts) return

  let inputOptions = opts
  if (typeof opts === 'string') { inputOptions = { message: opts } }

  const appNotifyOptions: QNotifyCreateOptions = {
    color: 'negative',
    icon: 'cancel',
    position: 'top'
  }

  const fullOptions = Object.assign(appNotifyOptions, inputOptions)
  Notify.create(fullOptions)
}

/**
 * 通知成功
 * @param opts
 */
export function notifySuccess (opts: QNotifyCreateOptions | string | undefined): void {
  if (!opts) return

  let inputOptions = opts
  if (typeof opts === 'string') { inputOptions = { message: opts } }

  const appNotifyOptions: QNotifyCreateOptions = {
    color: 'positive',
    icon: 'check_circle',
    position: 'top'
  }

  const fullOptions = Object.assign(appNotifyOptions, inputOptions)
  Notify.create(fullOptions)
}

/**
 * 确认操作
 * @param title
 * @param message
 */
export async function confirmOperation (title: string, message: string): Promise<boolean> {
  return new Promise((resolve) => {
    Dialog.create({
      title,
      message,
      ok: {
        color: 'secondary',
        icon: 'check_circle',
        label: '确认',
        tooltip: '确认操作',
        size: 'md',
        dense: true
      },
      cancel: {
        color: 'negative',
        icon: 'cancel',
        label: '取消',
        tooltip: '取消操作',
        size: 'md',
        dense: true
      }
    }).onOk(() => {
      resolve(true)
    }).onCancel(() => {
      resolve(false)
    }).onDismiss(() => {
      resolve(false)
    })
  })
}
