import { useSendEmailHub } from 'src/compositions/signalR'
import { notifyAny } from 'src/utils/notify'

export interface INotifyMessage {
  message?: string,
  type: 'success' | 'error' | 'info' | 'warning',
  title?: string
}

/**
 * 从服务器接收通知
 */
export function useNotifyRegister () {
  function receivedNotify (message: INotifyMessage) {
    console.log('receive message from server', message)

    notifyAny(message)
  }

  onMounted(() => {
    const signal = useSendEmailHub()
    signal.on('notify', receivedNotify)
  })

  onUnmounted(() => {
    const signal = useSendEmailHub()
    signal.off('notify', receivedNotify)
  })
}
