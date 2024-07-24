import { useUserInfoStore } from 'src/stores/user'

/**
 * 权限控制
 */
export function usePermission () {
  const store = useUserInfoStore()

  // 是否是管理员
  const isSuperAdmin = computed(() => {
    return hasPermission('*')
  })

  function hasPermission (access: string | string[]) {
    if (!Array.isArray(access)) access = [access]
    if (access.includes('*')) return true

    // 判断是否有权限
    return access.some(item => store.access.includes(item))
  }

  return {
    hasPermission,
    isSuperAdmin
  }
}
