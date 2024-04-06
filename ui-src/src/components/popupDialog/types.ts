import { IFunctionResult } from 'src/types'

/* eslint-disable @typescript-eslint/no-explicit-any */
export enum PopupDialogFieldType {
  text = 'text',
  textarea = 'textarea',
  boolean = 'boolean',
  selectOne = 'selectOne',
  selectMany = 'selectMany',
  date = 'date',
  password = 'password',
}

/**
 * 弹出菜单项
 */
export interface IPopupDialogField {
  type: PopupDialogFieldType,
  name: string, // 用于返回值的字段名
  label: string, // 显示的名称
  placeholder?: string, // 占位内容
  value?: object | string | number, // 默认值
  options?: Array<{ label: string, value: string | number }>, // 选项
  icon?: string, // 图标
  required?: boolean, // 是否必须
  validate?: (value: any) => Promise<IFunctionResult>, // 验证函数
  tooltip?: string, // 提示
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
  validate?: (data: Record<string, any>) => Promise<IFunctionResult>,
  // 窗体保持
  persistent?: boolean,
  // ok 最后执行的逻辑
  onOkMain?: (params: Record<string, any>) => Promise<void | boolean>,
}

/**
 * 对话框返回的结果
 */
export interface IDialogResult extends IFunctionResult {
  data: Record<string, any>
}
