import CryptoJS from 'crypto-js'

/**
 * 使用 ase 加密
 * @param key 是一个16进制的字符串
 * @param iv 是一个16进制的字符串
 * @param data 是一个字符串或者对象
 * @returns
 */
export function aes (key: string, iv: string, data: string | object) {
  if (typeof data === 'object') {
    // 如果传入的data是json对象，先转义为json字符串
    try {
      data = JSON.stringify(data)
    } catch (error) {
      console.log('error:', error)
    }
  }
  // 统一将传入的字符串转成UTF8编码
  const dataHex = CryptoJS.enc.Utf8.parse(data as string) // 需要加密的数据
  const keyHex = CryptoJS.enc.Hex.parse(key) // 秘钥
  const ivHex = CryptoJS.enc.Hex.parse(iv) // 偏移量
  const encrypted = CryptoJS.AES.encrypt(dataHex, keyHex, {
    iv: ivHex,
    mode: CryptoJS.mode.CBC, // 加密模式
    padding: CryptoJS.pad.Pkcs7
  })
  const encryptedVal = encrypted.ciphertext.toString()
  return encryptedVal //  返回加密后的值
}

/**
 * 解密数据
 * @param key
 * @param iv
 * @param ciphertext
 * @returns
 */
export function deAes (key: string, iv: string, ciphertext: string) {
  // console.log('解密：', key, iv, ciphertext)
  /*
    传入的key和iv需要和加密时候传入的key一致
  */

  // 统一将传入的字符串转成UTF8编码
  const encryptedHexStr = CryptoJS.enc.Hex.parse(ciphertext)
  ciphertext = CryptoJS.enc.Base64.stringify(encryptedHexStr)
  const keyHex = CryptoJS.enc.Hex.parse(key) // 秘钥
  const ivHex = CryptoJS.enc.Hex.parse(iv) // 偏移量
  const decrypt = CryptoJS.AES.decrypt(ciphertext, keyHex, {
    iv: ivHex,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7
  })
  const result = decrypt.toString(CryptoJS.enc.Utf8)
  // console.log('结果:', result)
  return result
}

/**
 * 计算 sha256
 * @param data
 * @returns
 */
export function sha256 (data: string) {
  return CryptoJS.SHA256(data).toString()
}

export interface IUploadFile extends File {
  __sha256?: string
  __key?: string,
  __sizeLabel?: string,
  __progressLabel?: string,
  __img?: string,
  __src?: string,
  __fileUsageId?: string | number
}

export interface FileSha256Callback {
  progressLabel: string
  process: number
  computed: number
  total: number
  file: IUploadFile,
  end: boolean
}

/**
 * 计算文件的 sha256
 * 参考：https://github1s.com/emn178/online-tools/blob/master/js/main.js#L37
 * https://blog.csdn.net/weixin_39364136/article/details/132538445
 * @param file
 * @param callback 若回调中修改界面，需要强制触发刷新
 * @param progressStep 回调进度的步长，默认为 5 %
 * @returns
 */
export function fileSha256 (file: File, callback?: (params: FileSha256Callback) => void, progressStep: number = 5): Promise<string> {
  console.log('getSha256:', file)
  return new Promise((resolve, reject) => {
    const reader = new FileReader()
    let start = 0
    const batch = 1024 * 1024 * 5
    const total = file.size
    const hashObject = CryptoJS.algo.SHA256.create()
    progressStep /= 100
    let progressDisplayLimit = 0
    function asyncUpdate () {
      if (start < total) {
        const process = start / total
        const progressLabel = '正在计算 hash 值...' + (process * 100).toFixed(2) + '%'
        const end = Math.min(start + batch, total)
        reader.readAsArrayBuffer(file.slice(start, end))
        start = end

        // 回调
        if (typeof callback === 'function' && progressDisplayLimit <= process) {
          progressDisplayLimit += progressStep
          const callbackResult = {
            progressLabel,
            process,
            computed: end,
            total,
            file,
            end: false
          }
          // console.log('fileSha256 callback:', callbackResult)
          callback(callbackResult)
        }
      } else {
        const sha256 = hashObject.finalize()
        console.log(`文件 ${file.name} sha256 值为：`, sha256.toString())

        if (typeof callback === 'function') {
          const callbackResult = {
            progressLabel: 'hash 已校验, 等待上传', // 重置为未上传的显示状态
            process: total,
            computed: total,
            total,
            file,
            end: true
          }
          callback(callbackResult)
        }
        resolve(sha256.toString())
      }
    }

    reader.onload = function () {
      // console.log('onload:', event, new Uint8Array(event.target.result))
      const arrayBuffer = reader.result as ArrayBuffer
      const wordArray = CryptoJS.lib.WordArray.create(arrayBuffer)
      // 更新哈希对象
      hashObject.update(wordArray)
      asyncUpdate()
    }

    asyncUpdate()

    reader.onerror = err => {
      reject({ ok: false, message: err })
    }
  })
}
