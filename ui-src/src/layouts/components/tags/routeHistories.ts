import { IRouteHistory } from './types'

export function useRouteHistories () {
  const routes: Ref<IRouteHistory[]> = ref([])
  // 监听 route 变化
  const route = useRoute()
  watch(route, () => {
    // 将其它路由设置为非激活状态
    routes.value.forEach((item) => {
      item.isActive = false
    })

    // 判断是否已存在
    const existIndex = routes.value.findIndex((item) => item.fullPath === route.fullPath)
    // 不存在时，添加
    if (existIndex < 0) {
      const routeTemp = {
        fullPath: route.fullPath,
        label: route.meta.label,
        name: route.name,
        isActive: true,
        icon: route.meta.icon,
        showCloseIcon: false
      } as IRouteHistory
      routes.value.push(routeTemp)
    } else {
      // 替换
      routes.value[existIndex].isActive = true
    }
  }, { immediate: true })

  return routes
}
