import { RouteRecordRaw } from 'vue-router'
import NormalLayout from 'layouts/normalLayout/normalLayout.vue'

// 静态 routes
export const constantRoutes: RouteRecordRaw[] = [
  {
    name: 'IndexHome',
    path: '/',
    component: NormalLayout,
    meta: {
      label: '首页',
      icon: 'home'
    },
    children: [
      {
        name: 'EmptyPage',
        path: 'index',
        meta: {
          label: '首页2',
          icon: 'home'
        },
        component: () => import('pages/IndexPage.vue')
      }
    ]
  },
  {
    name: 'user',
    path: '/user',
    component: NormalLayout,
    meta: {
      label: '用户',
      icon: 'info'
    },
    children: [
      {
        name: 'my',
        path: 'me',
        meta: {
          icon: 'menu',
          label: '我的'
        },
        component: () => import('pages/IndexPage.vue')
      }
    ]
  },

  // Always leave this as last one,
  // but you can also remove it
  // 异常处理
  {
    name: 'exception',
    path: '/:catchAll(.*)*',
    meta: {
      label: '异常',
      icon: 'error',
      hideMenu: true, // 在菜单中隐藏
      hideTag: true // 在标签中隐藏
    },
    component: () => import('pages/ErrorNotFound.vue')
  }
]

// 根据权限显示的 routes
export const dynamicRoutes: RouteRecordRaw[] = []
