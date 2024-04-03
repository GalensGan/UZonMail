/* eslint-disable @typescript-eslint/no-explicit-any */
export enum PopupDialogFieldType {
  text = 'text',
  textarea = 'textarea',
  boolean = 'boolean',
  selectOne = 'selectOne',
  selectMany = 'selectMany',
  date = 'string'
}

/**
 * 弹出菜单项
 */
export interface IPopupDialogField {
  type: PopupDialogFieldType,
  name: string, // 用于返回值的字段名
  label: string, // 显示的名称
  placeHolder?: string, // 占位内容
  value?: object | string | number, // 默认值
  options?: Array<{ label: string, value: string | number }>, // 选项
}

/**
 * 弹出框参数
 */
export interface IPopupDialogParams {
  title?: string, // 标题
  // 字段定义
  fields: Array<IPopupDialogField>,
  // 数据源
  dataSet?: Record<string, Array<string | number | boolean | object> | Promise<any[]>>,
  // 用于数据验证
  validate?: (data: object) => Promise<boolean>,
  // 窗体保持
  persist: boolean
}

/**
 * 对话框返回的结果
 */
export interface IDialogResult {
  ok: boolean,
  message?: string,
  data?: object
}
