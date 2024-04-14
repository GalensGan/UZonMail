/**
 * 路由历史
 */
export interface IRouteHistory {
  fullPath: string,
  name: string,
  label: string,
  isActive: boolean,
  icon: string,
  noCache: boolean,
  showCloseIcon: boolean
}
