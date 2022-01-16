import { Dialog, Notify } from 'quasar'

function okCancle(title, message, options) {
  const initOptions = {
    title,
    message,
    ok: {
      dense: true,
      color: 'primary'
    },
    cancel: {
      dense: true,
      color: 'negative'
    },
    persistent: true
  }

  if (options) {
    Object.assign(initOptions, options)
  }

  const confirm = new Promise((resolve, reject) => {
    Dialog.create(initOptions)
      .onOk(data => {
        resolve(data || true)
      })
      .onCancel(() => {
        resolve(false)
      })
  })

  return confirm
}

/**
 * 提示错误
 * @param {*} message
 */
function notifyError(message) {
  Notify.create({
    message,
    icon: 'error',
    color: 'negative',
    position: 'top'
  })
}

/**
 * 提示成功
 * @param {*} message
 */
function notifySuccess(message) {
  Notify.create({
    message,
    icon: 'done',
    color: 'primary',
    position: 'top'
  })
}

export { okCancle, notifyError, notifySuccess }
