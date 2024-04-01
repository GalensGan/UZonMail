import { RouteRecordRaw } from 'vue-router'

// 为了支持更多路由的 meta 属性，我们需要扩展RouteRecordRaw
export type ExtendedRouteRecordRaw = RouteRecordRaw & {
  meta: {
    label: string
    icon: string,
    access?: string[],
    noTag?: boolean,
    noMenu?: boolean
  }
}
