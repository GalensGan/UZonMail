import { httpClient } from 'src/api//base/httpClient'

/**
 * 上传文件到静态目录
 * @param subPath
 * @param fileName 文件名传入时应规范命名，不要包含特殊字符
 * @param data
 * @returns
 */
export function uploadToStaticFile (subPath: string, fileName: string, data: Blob) {
  // 去掉文件名中的空格
  fileName = fileName.replace(/\s/g, '')

  const form = new FormData()
  form.append('subPath', subPath)
  form.append('file', data, fileName)
  return httpClient.post<string>('/file/upload-static-file', {
    data: form
  })
}
