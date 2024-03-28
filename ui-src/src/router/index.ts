import { route } from 'quasar/wrappers'
import {
  createMemoryHistory,
  createRouter,
  createWebHashHistory,
  createWebHistory
} from 'vue-router'

import { constantRoutes } from './routes'
import { useRoutesStore } from 'src/stores/routes'

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

  const Router = createRouter({
    scrollBehavior: () => ({ left: 0, top: 0 }),
    routes: constantRoutes,

    // Leave this as is and make changes in quasar.conf.js instead!
    // quasar.conf.js -> build -> vueRouterMode
    // quasar.conf.js -> build -> publicPath
    history: createHistory(process.env.VUE_ROUTER_BASE)
  })

  // 添加路由前置守卫
  Router.beforeEach((to, from, next) => {
    // 添加动态路由
    const routeStore = useRoutesStore()
    routeStore.addDynamicRoutes()
    next()
  })

  // 添加路由后置守卫
  Router.afterEach((to, from) => {
    console.log('路由后置守卫', to, from)
  })
  return Router
})
