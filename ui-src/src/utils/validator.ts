/**
 * 验证是否是邮箱
 * @param emailStr
 * @returns
 */
export function isEmail (emailStr: string) {
  const reg = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/
  return reg.test(emailStr)
}
