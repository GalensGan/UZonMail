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
      console.log('OK', model)
      resolve({ ok: true, data: model })
    }).onCancel(() => {
      console.log('Cancel')
      resolve({ ok: false })
    })
  })
}
