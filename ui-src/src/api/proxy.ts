import { httpClient } from 'src/api//base/httpClient'

export interface IProxy {
  id?: number
  name: string
  priority?: number
  emailMatch?: string
  description?: string
  isActive: boolean
  proxy: string
}

export function CreateProxy (proxyInfo: IProxy) {
  return httpClient.post<IProxy>('/file/upload-static-file', {
    data: proxyInfo
  })
}
