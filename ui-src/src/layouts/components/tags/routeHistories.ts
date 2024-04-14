import { IRouteHistory } from './types'

export function useRouteHistories () {
  const routes: Ref<IRouteHistory[]> = ref([])
  // 监听 route 变化
  const route = useRoute()
  watch(route, (newRoute) => {
    // 隐藏标签的路由，不显示
    if (newRoute.meta.noTag) return

    // 将其它路由设置为非激活状态
    routes.value.forEach((item) => {
      item.isActive = false
    })

    // 判断是否已存在
    const existIndex = routes.value.findIndex((item) => item.fullPath === newRoute.fullPath)
    // 不存在时，添加
    if (existIndex < 0) {
      const routeTemp = {
        fullPath: newRoute.fullPath,
        label: newRoute.meta.label,
        name: newRoute.name,
        isActive: true,
        icon: newRoute.meta.icon,
        // 专门用于标签页的属性
        showCloseIcon: false,
        noCache: newRoute.meta.noCache
      } as IRouteHistory
      routes.value.push(routeTemp)
    } else {
      // 替换
      routes.value[existIndex].isActive = true
    }
  }, { immediate: true })

  return routes
}
