import { defineStore } from 'pinia'
import { useStorage } from '@vueuse/core'
import { IUserInfo } from './types'

// options 方式定义
export const useUserInfoStore = defineStore('userInfo', {
  state: () => ({
    token: useStorage('token', '').value,
    access: useStorage('access', []).value as string[],
    userInfo: useStorage('userInfo', {
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
      useStorage('token', token)
    },

    setUserInfo (userInfo: IUserInfo) {
      this.userInfo = userInfo
      useStorage('userInfo', userInfo)
    },

    setAccess (access: string[]) {
      this.access = access
      useStorage('access', access)
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
    }
  }
})
