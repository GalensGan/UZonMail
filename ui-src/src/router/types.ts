import { RouteRecordRaw } from 'vue-router'

// 为了支持更多路由的 meta 属性，我们需要扩展RouteRecordRaw
export type extendedRouteRecordRaw = RouteRecordRaw | {
  meta:{

  }
}
