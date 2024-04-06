import { IDialogResult, IPopupDialogParams } from './types'
import { Dialog } from 'quasar'
import lowCodeForm from './lowCodeForm.vue'

/**
 * 弹出对话框
 */
export async function showDialog (dialogParams: IPopupDialogParams): Promise<IDialogResult> {
  /**
  * 显示对话框并返回结果
  */

  return new Promise((resolve) => {
    Dialog.create({
      component: lowCodeForm,
      componentProps: dialogParams
    }).onOk((model) => {
      // console.log('OK', model)
      resolve({ ok: true, data: model })
    }).onCancel(() => {
      // console.log('Cancel')
      resolve({ ok: false, data: {} })
    }).onDismiss(() => {
      // console.log('Dismiss')
      resolve({ ok: false, data: {} })
    })
  })
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export async function showComponentDialog (component: Component, componentProps?: any): Promise<IDialogResult> {
  return new Promise((resolve) => {
    Dialog.create({
      component,
      componentProps
    }).onOk((model) => {
      resolve({ ok: true, data: model })
    }).onCancel(() => {
      resolve({ ok: false, data: {} })
    }).onDismiss(() => {
      resolve({ ok: false, data: {} })
    })
  })
}
