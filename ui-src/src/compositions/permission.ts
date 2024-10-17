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

  const hasPermission = store.hasPermission
  const hasPermissionOr = store.hasPermissionOr

  /**
   * 是否有专业版权限
   * @returns
   */
  function hasProfessionAccess () {
    if (!store.hasProPlugin) return false
    if (hasEnterpriseAccess()) return true
    return hasPermissionOr(['professional'])
  }

  /**
   * 是否有企业版权限
   * @returns
   */
  function hasEnterpriseAccess () {
    return hasPermission('enterprise')
  }

  return {
    hasPermission,
    hasPermissionOr,
    isSuperAdmin,
    hasProfessionAccess,
    hasEnterpriseAccess
  }
}