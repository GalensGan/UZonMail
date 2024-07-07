/* eslint-disable @typescript-eslint/no-explicit-any */
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { notifyError } from 'src/utils/dialog'
import * as XLSX from 'xlsx'
import { PopupDialogFieldType } from 'src/components/popupDialog/types'
import CryptoJS from 'crypto-js'

export interface ISelectFileResult {
  ok: boolean,
  data: string | ArrayBuffer | undefined | null,
  files?: FileList
}

/**
 * 选择文件
 * @param multiple
 * @param accept
 */
export function selectFile (multiple: boolean = false, accept: string = ''): Promise<ISelectFileResult> {
  const promise = new Promise<ISelectFileResult>((resolve, reject) => {
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
        notifyError('未找到文件')
        // 删除 input
        // inputElement.remove()
        return reject({
          ok: false,
          message: '未找到文件'
        })
      }

      const file = files[0]
      // 读取数据
      const reader = new FileReader()
      reader.onload = e => {
        // 读取workbook
        const buffer = e.target?.result
        // 删除 input
        // inputElement.remove()
        resolve({
          ok: true,
          data: buffer,
          files
        })
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
            reject({
              ok: false,
              message: '取消选择文件'
            })
          }
        }, 500)
      },
      { once: true }
    )
    // 移除 input
    inputElement.remove()
  })

  return promise
}

/**
 * 打开文件选择器
 * @param accept
 * @param multiple
 * @returns
 */
export async function openFileSelector (multiple: boolean = false, accept: string = ''): Promise<boolean | string | ArrayBuffer | undefined | null> {
  const { data } = await selectFile(multiple, accept)
  return data
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

// #region  Excel 相关操作

// 类型定义
/**
 * Excel 列映射
 */
export interface IExcelColumnMapper {
  // 对应表格中的列标题名
  headerName: string,
  // 对应数据中的字段名
  fieldName: string,
  // 格式化函数
  format?: (value: any) => any,
  // 过滤函数
  filter?: (value: any) => boolean,
  // 是否必须
  required?: boolean
}

export interface IExcelMapperParams {
  // 映射
  mappers?: IExcelColumnMapper[],
  format?: (value: Record<string, any>) => Record<string, any>,
  filter?: (value: Record<string, any>) => boolean
}

export interface IExcelReaderParams extends IExcelMapperParams {
  sheetIndex: number,
  // 是否要选择 sheet
  selectSheet?: boolean,
  // 数字类型保留的小数位，默认最多 5 位
  numberDecimalCount?: number
}

export interface IExcelWriterParams extends IExcelMapperParams {
  // 名字应包含后缀，比如 .xlsx
  fileName: string,
  sheetName?: string
}

/**
 * 读取 excel 文件，会同时返回文件名和文件内容
 * @param params
 * @returns
 */
export async function readExcelCore (params: IExcelReaderParams): Promise<{ data: Record<string, any>[], files?: FileList, sheetName: string }> {
  // 打开文件
  const { data: buffer, files } = await selectFile()
  console.log('readExcelCore:', buffer, files)
  const workbook = XLSX.read(buffer, { type: 'buffer' })

  // 打开选择框
  if (params.selectSheet && workbook.SheetNames.length > 1) {
    const { ok, data } = await showDialog<Record<string, any>>({
      title: '指定 Sheet',
      fields: [
        {
          name: 'sheetName',
          type: PopupDialogFieldType.selectOne,
          label: '请选择 Sheet',
          value: workbook.SheetNames[0],
          options: workbook.SheetNames
        }
      ],
      oneColumn: true
    })
    if (!ok) {
      return {
        data: [],
        files,
        sheetName: ''
      }
    }
    params.sheetIndex = workbook.SheetNames.indexOf(data.sheetName)
  }

  const sheetName = workbook.SheetNames[params.sheetIndex]
  const worksheet = workbook.Sheets[sheetName]
  const rowsData = XLSX.utils.sheet_to_json(worksheet) as Record<string, any>[]
  // 对数据进行处理，主要是浮点数问题
  const decimalCount = params.numberDecimalCount === undefined ? 5 : params.numberDecimalCount
  rowsData.forEach(row => {
    for (const key of Object.keys(row)) {
      const value = row[key]
      if (typeof value === 'number') {
        row[key] = Number(value.toFixed(decimalCount))
      }
    }
  })
  // 将 mappers 转换成对象
  const mapper: Record<string, IExcelColumnMapper> = {}
  if (params.mappers) {
    params.mappers.forEach(item => {
      mapper[item.headerName] = item
    })
  }

  // 对每行数据进行处理
  const formattedRows = []
  for (const row of rowsData) {
    // 对数据进行转换
    let formattedRow: Record<string, any> = {}
    for (const key of Object.keys(row)) {
      const value = row[key]
      const map = mapper[key]
      if (map) {
        // 先格式化，再过滤
        const formattedValue = map.format ? map.format(value) : value

        // 过滤
        if (map.filter && !map.filter(formattedValue)) {
          continue
        }

        // 判断是否存在
        if (map.required) {
          if (formattedValue === null || formattedValue === undefined || formattedValue === '') {
            notifyError(`字段 ${map.headerName} 不能为空`)
            throw new Error(`字段 ${map.fieldName} 不能为空`)
          }
        }
        formattedRow[map.fieldName] = formattedValue
      } else {
        formattedRow[key] = value
      }
    }

    // 对行进行格式化
    if (params.format) {
      formattedRow = params.format(formattedRow)
    }

    // 对行进行过滤
    if (params.filter && !params.filter(formattedRow)) {
      continue
    }

    formattedRows.push(formattedRow)
  }

  return {
    data: formattedRows,
    files,
    sheetName
  }
}

/**
 * 读取 excel
 * @param params
 */
export async function readExcel (params: IExcelReaderParams) {
  // 打开文件
  const { data: results } = await readExcelCore(params)
  return results
}

/**
 * 导出 Excel
 * @param params
 */
export async function writeExcel (rows: any[], params: IExcelWriterParams) {
  // 创建一个新的工作簿
  const newWorkbook = XLSX.utils.book_new()

  // 将 mappers 转换成对象
  const mapper: Record<string, IExcelColumnMapper> = {}
  if (params.mappers) {
    params.mappers.forEach(item => {
      mapper[item.fieldName] = item
    })
  }

  const results: any[] = []
  let rowIndex = -1
  for (const row of rows) {
    rowIndex++
    // 对数据进行转换
    let formattedRow: Record<string, any> = {}
    for (const key of Object.keys(row)) {
      const value = row[key]
      const map = mapper[key]
      if (map) {
        // 先格式化，再过滤
        const formattedValue = map.format ? map.format(value) : value

        // 过滤
        if (map.filter && !map.filter(formattedValue)) {
          continue
        }

        // 判断是否存在
        if (map.required) {
          if (formattedValue === null || formattedValue === undefined || formattedValue === '') {
            notifyError(`第 ${rowIndex} 行数据中，${map.headerName} 列不能为空`)
            throw new Error(`列 ${map.headerName} 不能为空`)
          }
        }

        formattedRow[map.headerName] = formattedValue
      } else {
        formattedRow[key] = value
      }
    }

    // 对行进行格式化
    if (params.format) {
      formattedRow = params.format(formattedRow)
    }

    // 对行进行过滤
    if (params.filter && !params.filter(formattedRow)) {
      continue
    }

    results.push(formattedRow)
  }

  // 创建一个新的工作表
  const newWorksheet = XLSX.utils.json_to_sheet(results)
  // 将工作表添加到工作簿
  XLSX.utils.book_append_sheet(newWorkbook, newWorksheet, params.sheetName || 'Sheet1')
  // 写入文件
  XLSX.writeFile(newWorkbook, params.fileName)
}
// #endregion

// #region 文件上传相关

export interface IUploadResult {
  // 该字段用于在初始化时使用
  __fileName?: string,
  __sha256?: string
  __key?: string,
  __sizeLabel?: string,
  __progressLabel?: string,
  __fileUsageId?: string | number
}

/**
 * 上传的文件
 */
export interface IUploadFile extends IUploadResult, File {
  __img?: string,
  __src?: string,
}

/**
 * hash 计算回调
 */
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
// #endregion

// #region 文件保存相关操作
/**
 * 保存字符串到文件
 * @param fileName
 * @param content
 */
export async function saveStringToFile (fileName: string, content: string) {
  const blob = new Blob([content], { type: 'text/plain' })
  const url = URL.createObjectURL(blob)
  await saveUrlToFile(fileName, url)
}

/**
 * 保存 url 到文件
 * @param fileName
 * @param fileUrl
 */
export async function saveUrlToFile (fileName: string, fileUrl: string) {
  const aLink = document.createElement('a')
  aLink.href = fileUrl
  aLink.download = fileName
  aLink.click()
  // 删除 alink
  aLink.remove()
}
// #endregion
