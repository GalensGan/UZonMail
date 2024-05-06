/* eslint-disable @typescript-eslint/no-explicit-any */
import * as signalR from '@microsoft/signalr'
import { useConfig } from 'src/config'
import { useUserInfoStore } from 'src/stores/user'

export interface ISignalRs {
  sendingProgressHub?: signalR.HubConnection
}

const signalRs: ISignalRs = {
}

/**
 * 获取发送邮件的 signalR 连接
 * 使用 on 注册事件，服务器会调用该事件并发送通知
 */
export function useSendEmailHub () {
  if (signalRs.sendingProgressHub) return signalRs.sendingProgressHub

  // 新建一个连接
  const config = useConfig()
  const userInfoStore = useUserInfoStore()

  const url = `${config.baseUrl}${config.signalRHub}`
  const signal = new signalR.HubConnectionBuilder()
    .withUrl(url, {
      skipNegotiation: true,
      transport: signalR.HttpTransportType.WebSockets,
      accessTokenFactory () {
        return userInfoStore.token
      }
    })
    .configureLogging(signalR.LogLevel.Information)
    .build()
  // 前端使用 on 来接收消息，若要注销，使用 off 即可
  // signal.on('SendAll', (res) => {
  //   console.log(res, '收到消息')
  // })
  signal.start().then(() => {
    // if (window.Notification) {
    //   if (Notification.permission === 'granted') {
    //     console.log('允许通知')
    //   } else if (Notification.permission !== 'denied') {
    //     console.log('需要通知权限')
    //     Notification.requestPermission((permission) => { console.log('权限通知', permission) })
    //   } else if (Notification.permission === 'denied') {
    //     console.log('拒绝通知')
    //   }
    // } else {
    //   console.error('浏览器不支持Notification')
    // }
    console.log('连接成功')
  })
  signal.onclose((err) => {
    console.log('连接已经断开 执行函数onclose', err)
    signalRs.sendingProgressHub = undefined
  })

  signalRs.sendingProgressHub = signal
  return signal
}

/**
 * 向服务器订阅通信
 * @param methodName
 * @param newMethod
 * @returns
 */
export function subscribeOne (methodName: string, newMethod: (...args: any[]) => any): void {
  const hub = useSendEmailHub()
  if (!hub) return

  onMounted(async () => {
    hub.on(methodName, newMethod)
  })

  onUnmounted(() => {
    hub.off(methodName, newMethod)
  })
}

/**
 * 只订阅一次
 */
export function subscribeOnce (methodName: string, newMethod: (...args: any[]) => any): void {
  const hub = useSendEmailHub()
  if (!hub) return

  function methodCore (...args: any[]): any {
    newMethod(...args)
    hub.off(methodName, methodCore)
  }

  onMounted(async () => {
    hub.on(methodName, methodCore)
  })

  onUnmounted(() => {
    hub.off(methodName, methodCore)
  })
}

export interface ISubscribeInfo {
  methodName: string
  newMethod: (...args: any[]) => any
}

export function subscribeMany (subscribeInfos: ISubscribeInfo[]) {
  const hub = useSendEmailHub()
  if (!hub) return

  onMounted(async () => {
    for (const { methodName, newMethod } of subscribeInfos) {
      hub.on(methodName, newMethod)
    }
  })

  onUnmounted(() => {
    for (const { methodName, newMethod } of subscribeInfos) {
      hub.off(methodName, newMethod)
    }
  })
}
