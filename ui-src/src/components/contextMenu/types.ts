/* eslint-disable @typescript-eslint/no-explicit-any */
export interface IContextMenuItem {
  name: string,
  label: string,
  tooltip?: string | string[] | ((params?:object)=>Promise<string[]>),
  color?: string,
  onClick: (value: object) => void,
  vif?: (value: object) => void
}
