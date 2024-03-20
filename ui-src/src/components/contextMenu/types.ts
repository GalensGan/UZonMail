/* eslint-disable @typescript-eslint/no-explicit-any */
export interface IContextMenuItem {
  name: string,
  label: string,
  color?: string,
  onClick: (value: object) => void,
  vif?: (value: object) => void
}
