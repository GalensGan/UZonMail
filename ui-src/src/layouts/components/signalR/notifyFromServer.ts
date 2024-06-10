import { subscribeOne } from 'src/signalR/signalR'
import { UzonMailClientMethods } from 'src/signalR/types'
import { notifyAny } from 'src/utils/dialog'

export interface INotifyMessage {
  message?: string,
  type: 'success' | 'error' | 'info' | 'warning',
  title?: string
}

/**
 * 从服务器接收通知
 */
export async function useNotifyRegister () {
  function receivedNotify (message: INotifyMessage) {
    console.log('receive message from server', message)

    notifyAny(message)
  }

  subscribeOne(UzonMailClientMethods.notify, receivedNotify)
}
