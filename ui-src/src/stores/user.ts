import { defineStore } from 'pinia'
import { useSessionStorage } from '@vueuse/core'
import { IUserInfo } from './types'
import { useRouter } from 'src/router/index'

// options 方式定义
export const useUserInfoStore = defineStore('userInfo', {
  state: () => ({
    token: useSessionStorage('token', '').value,
    access: useSessionStorage('access', []).value as string[],
    userInfo: useSessionStorage('userInfo', {
      userId: '',
      userName: '',
      avatar: ''
    } as IUserInfo).value
  }),
  getters: {
    userId: (state) => state.userInfo.userId,
    userName: (state) => state.userInfo.userName,
    userAvatar: (state) => state.userInfo.avatar
  },
  actions: {
    setToken (token: string) {
      this.token = token
      // 保存到 session 中，方便刷新后恢复
      const tokenSession = useSessionStorage('token', '')
      console.log('setToken: ', token, tokenSession.value)
      tokenSession.value = token
    },

    setUserInfo (userInfo: IUserInfo) {
      this.userInfo = userInfo
      const userInfoSession = useSessionStorage('userInfo', userInfo)
      userInfoSession.value = userInfo
    },

    setAccess (access: string[]) {
      this.access = access
      const accessSession = useSessionStorage('access', access)
      accessSession.value = access
    },

    setUserLoginInfo (userInfo: IUserInfo, token: string, access: string[]) {
      this.setUserInfo(userInfo)
      this.setToken(token)
      this.setAccess(access)
    },

    /**
     * 判断是否有权限
     * @param targetAccess
     */
    hasPermission (targetAccess: string[] | string) {
      // * 表示超管权限
      if (this.access.includes('*')) return true

      if (!targetAccess || targetAccess.length === 0) return false
      if (typeof targetAccess === 'string') targetAccess = [targetAccess]
      return this.access.some(x => targetAccess.includes(x))
    },

    // 退出登陆
    logout () {
      // 重置数据
      const tokenSession = useSessionStorage('token', '')
      const accessSession = useSessionStorage('access', [])
      tokenSession.value = null
      accessSession.value = null

      // 重定向到登陆页面
      const router = useRouter()
      router.push('/login')
    }
  }
})
