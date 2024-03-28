import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import { useUserInfoStore } from './user'
import { dynamicRoutes } from 'src/router/routes'
import { ExtendedRouteRecordRaw } from 'src/router/types'

// 过滤动态路由
function filterDynamicRouesByAccess (routes: ExtendedRouteRecordRaw[]): ExtendedRouteRecordRaw[] {
  if (!routes || routes.length === 0) return []
  const results: ExtendedRouteRecordRaw[] = []
  const userInfoStore = useUserInfoStore()
  for (const route of routes) {
    if (route.meta?.access) {
      if (!userInfoStore.hasPermission(route.meta.access)) {
        continue
      }
    }

    // 保存
    results.push(route)

    // 判断子级权限
    const children = filterDynamicRouesByAccess(route.children as ExtendedRouteRecordRaw[])
    route.children = children
  }
  return results
}

// options 方式定义
export const useRoutesStore = defineStore('routes', {
  state: () => ({
    isAddedDynamicRoutes: false
  }),
  actions: {
    addDynamicRoutes () {
      console.log('addDynamicRoutes', this.isAddedDynamicRoutes)
      if (this.isAddedDynamicRoutes) return
      this.isAddedDynamicRoutes = true

      // 动态添加路由
      const accessRoutes = filterDynamicRouesByAccess(dynamicRoutes)
      console.log('动态路由', accessRoutes)
      const router = useRouter()
      accessRoutes.forEach(route => {
        router.addRoute(route)
      })
    }
  }
})
