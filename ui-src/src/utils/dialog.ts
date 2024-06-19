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
 * 根据传递的 type 来智能识别消息类型
 * @param opts
 * @returns
 */
export function notifyAny (opts: QNotifyCreateOptions | string | undefined): void {
  if (!opts) return
  if (typeof opts === 'string') opts = { message: opts, type: 'success' }

  switch (opts.type) {
    case 'success':
      notifySuccess(opts)
      break
    case 'error':
      notifyError(opts)
      break
    default:
      Notify.create(opts)
  }
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

/**
 * 显示 html 内容
 * @param title
 * @param html
 * @returns
 */
export function showHtmlDialog (title: string, html: string) {
  return new Promise((resolve) => {
    Dialog.create({
      title,
      message: html,
      html: true,
      ok: {
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

// #region 对 components/popupDialog/PopupDialog.ts 进行导出，统一弹窗调用位置
export { showDialog, showComponentDialog } from 'src/components/popupDialog/PopupDialog'
// #endregion
