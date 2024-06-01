/* eslint-disable @typescript-eslint/no-explicit-any */
export interface IContextMenuItem {
  name: string,
  label: string,
  tooltip?: string | string[] | ((params?: Record<string, any>) => Promise<string[]>),
  color?: string,
  icon?: string, // 图标
  // 当返回 false 时，右键菜单不会退出
  onClick: (value: Record<string, any>) => Promise<void | boolean>,
  vif?: (value: Record<string, any>) => boolean,
}
