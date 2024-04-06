import { notifyError } from './notify'

/**
 * 打开文件选择器
 * @param accept
 * @param multiple
 * @returns
 */
export function openFileSelector (multiple: boolean = false, accept: string = ''): Promise<boolean | string | ArrayBuffer | undefined | null> {
  const promise = new Promise<boolean | string | ArrayBuffer | undefined | null>((resolve, reject) => {
    const inputElement = document.createElement('input')
    inputElement.type = 'file'
    inputElement.accept = accept
    inputElement.multiple = multiple
    let fileCancel = true

    const callback = (data: Event) => {
      fileCancel = false

      // 获取file文件
      const files = (data.target as HTMLInputElement).files
      if (!files || files.length < 1) {
        notifyError('没有找到文件')
        // 删除 input
        // inputElement.remove()
        return reject('没有找到文件')
      }

      const file = files[0]
      // 读取数据
      const reader = new FileReader()
      reader.onload = e => {
        // 读取workbook
        const buffer = e.target?.result
        // 删除 input
        // inputElement.remove()
        resolve(buffer)
      }
      reader.readAsArrayBuffer(file)
    }
    inputElement.addEventListener('change', callback)
    inputElement.dispatchEvent(new MouseEvent('click'))
    // 模拟取消事件
    window.addEventListener(
      'focus',
      () => {
        setTimeout(() => {
          if (fileCancel) {
            reject(false)
          }
        }, 300)
      },
      { once: true }
    )
    // 移除 input
    inputElement.remove()
  })

  return promise
}

/**
 * buffer 转 blob
 * @param buffer
 * @returns
 */
export function bufferToBlob (buffer: ArrayBuffer): Blob {
  return new Blob([buffer])
}

/**
 * buffer 转 base64 图片
 * @param buffer
 * @returns
 */
export function bufferToBase64Png (buffer: ArrayBuffer): string {
  let binary = ''
  const bytes = new Uint8Array(buffer)
  for (let len = bytes.byteLength, i = 0; i < len; i++) {
    binary += String.fromCharCode(bytes[i])
  }
  const base64 = 'data:image/png;base64,' + window.btoa(binary)
  return base64
}
