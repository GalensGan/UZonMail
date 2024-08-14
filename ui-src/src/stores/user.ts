import { defineStore } from 'pinia'
import { useSessionStorage } from '@vueuse/core'
import { IUserInfo } from './types'
import { useRouter } from 'src/router/index'
import logger from 'loglevel'

// options 方式定义
export const useUserInfoStore = defineStore('userInfo', {
  state: () => ({
    token: useSessionStorage('token', '').value,
    access: useSessionStorage('access', []).value as string[],
    userId: useSessionStorage('userId', '').value,
    userName: useSessionStorage('userName', '').value,
    avatar: useSessionStorage('avatar', '').value,
    secretKey: useSessionStorage('secretKey', '').value
  }),
  getters: {
    userInfo: (state) => {
      return {
        userId: state.userId,
        userName: state.userName,
        avatar: state.avatar
      }
    },
    userAvatar: (state) => {
      // 判断是否包含 http，若不包含，添加后端的 http
      const avatar = state.avatar
      if (!avatar) return avatar

      if (avatar.startsWith('http')) return avatar
      const url = new URL(avatar, process.env.BASE_URL?.replace('/api/v1', ''))
      return url.toString()
    },
    isAdmin: (state) => {
      // 目前使用 * 或者 admin 来表示超管权限
      return state.access.includes('*') || state.userId === 'admin'
    },
    // smtp 加密解密密钥
    smtpPasswordSecretKeys: (state) => {
      const key = state.secretKey
      if (!key || key.length < 16) return [key, key]
      return [key, key.substring(0, 16)]
    }
  },
  actions: {
    setToken (token: string) {
      this.token = token
      // 保存到 session 中，方便刷新后恢复
      const tokenSession = useSessionStorage('token', '')
      logger.debug('[UserStore] setToken: ', token, tokenSession.value)
      tokenSession.value = token
    },

    setUserInfo (userInfo: IUserInfo) {
      this.userId = userInfo.userId
      this.userName = userInfo.userName
      this.avatar = userInfo.avatar

      useSessionStorage('userId', this.userId).value = this.userId
      useSessionStorage('userName', this.userName).value = this.userName
      useSessionStorage('avatar', this.avatar).value = this.avatar
    },

    setAccess (access: string[]) {
      this.access = access
      const accessSession = useSessionStorage('access', access)
      accessSession.value = access
    },

    setUserLoginInfo (userInfo: IUserInfo, token: string, access: string[]) {
      logger.debug('[UserStore] setUserLoginInfo')
      this.setUserInfo(userInfo)
      this.setToken(token)
      this.setAccess(access)
    },

    // 保存用户自己的密钥
    setSecretKey (key: string) {
      this.secretKey = key
      useSessionStorage('secretKey', this.secretKey).value = key
    },

    /**
     * 判断是否有权限
     * @param targetAccess 所有权限都满足时，才返回 true
     */
    hasPermission (targetAccess: string[] | string) {
      if (!targetAccess || targetAccess.length === 0) return false
      if (typeof targetAccess === 'string') targetAccess = [targetAccess]

      return targetAccess.every(x => this.access.includes(x))
    },

    /**
     * 是否有拒绝权限
     * @param targetDenies 只要有一个权限，就返回 true
     */
    hasDenies (targetDenies: string[] | string) {
      if (!targetDenies || targetDenies.length === 0) return false
      if (typeof targetDenies === 'string') targetDenies = [targetDenies]
      return targetDenies.some(x => this.access.includes(x))
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
    },

    /**
     * 更新用户头像
     * @param string
     * @param avatarUrl
     */
    updateUserAvatar (avatarUrl: string) {
      this.avatar = avatarUrl
      useSessionStorage('avatar', avatarUrl).value = avatarUrl
    }
  }
})
