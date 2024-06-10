import { route } from 'quasar/wrappers'
import {
  Router,
  createMemoryHistory,
  createRouter,
  createWebHashHistory,
  createWebHistory
} from 'vue-router'

import { constantRoutes } from './routes'
import { useRoutesStore } from 'src/stores/routes'
import { useUserInfoStore } from 'src/stores/user'

let router: Router
export function useRouter () {
  return router
}

/*
 * If not building with SSR mode, you can
 * directly export the Router instantiation;
 *
 * The function below can be async too; either use
 * async/await or return a Promise which resolves
 * with the Router instance....
 */

export default route(function (/* { store, ssrContext } */) {
  const createHistory = process.env.SERVER
    ? createMemoryHistory
    : process.env.VUE_ROUTER_MODE === 'history'
      ? createWebHistory
      : createWebHashHistory

  router = createRouter({
    scrollBehavior: () => ({ left: 0, top: 0 }),
    routes: constantRoutes,

    // Leave this as is and make changes in quasar.conf.js instead!
    // quasar.conf.js -> build -> vueRouterMode
    // quasar.conf.js -> build -> publicPath
    history: createHistory(process.env.VUE_ROUTER_BASE)
  })

  const userInfoStore = useUserInfoStore()
  // 添加路由前置守卫
  router.beforeEach((to, from, next) => {
    // console.log('userInfoStore: ', userInfoStore)
    if (!userInfoStore.token && to.path !== '/login') {
      // 跳转到登陆界面
      next('/login')
      return
    }

    // console.log('路由前置守卫', to, from)
    // 添加动态路由
    const routeStore = useRoutesStore()
    if (routeStore.addDynamicRoutes()) {
      // console.log('添加动态路由')
      // 重新导航
      router.replace(to)
      return
    }
    next()
  })

  // 添加路由后置守卫
  // Router.afterEach((to, from) => {
  //   console.log('路由后置守卫', to, from)
  // })
  return router
})
