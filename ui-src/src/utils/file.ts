/* eslint-disable @typescript-eslint/no-explicit-any */
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { notifyError } from './notify'
import * as XLSX from 'xlsx'
import { PopupDialogFieldType } from 'src/components/popupDialog/types'

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
        notifyError('没有找到文件')
        // 删除 input
        // inputElement.remove()
        return reject({
          ok: false
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
              ok: false
            })
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
  selectSheet?: boolean
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
  const workbook = XLSX.read(buffer, { type: 'buffer' })

  // 打开选择框
  if (params.selectSheet && workbook.SheetNames.length > 1) {
    const { ok, data } = await showDialog<Record<string, any>>({
      fields: [
        {
          name: 'sheetName',
          type: PopupDialogFieldType.selectOne,
          label: '请选择 Sheet',
          value: workbook.SheetNames[0],
          options: workbook.SheetNames
        }
      ]
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

  // 将 mappers 转换成对象
  const mapper: Record<string, IExcelColumnMapper> = {}
  if (params.mappers) {
    params.mappers.forEach(item => {
      mapper[item.headerName] = item
    })
  }

  // 对每行数据进行处理
  const results = []
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

    results.push({
      data: formattedRow,
      files
    })
  }

  return {
    data: results,
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
