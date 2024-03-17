import { RouteRecordRaw } from 'vue-router'
import NormalLayout from 'layouts/normalLayout/normalLayout.vue'

const routes: RouteRecordRaw[] = [
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
    path: '/user',
    component: NormalLayout,
    meta: {
      label: '用户',
      icon: 'info'
    },
    children: [
      {
        path: 'person',
        meta: {
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
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue')
  }
]

export default routes
