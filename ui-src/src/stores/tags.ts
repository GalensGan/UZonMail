import { defineStore } from 'pinia'

// options 方式定义
export const useCounterStore = defineStore('counter', {
  state: () => ({
    tags: []
  }),
  actions: {
    updateRoute () {
      const route = useRoute()
      // 判断当前路由是否已经存在
      const isExist = this.tags.some(item => item.path === route.path)
    }
  }
})
