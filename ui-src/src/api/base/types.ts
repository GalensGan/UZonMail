export interface IHttpClientOptions {
  // 是否开启取消重复请求, 默认为 true
  cancelDuplicateRequest?: boolean
  // 是否开启loading层效果, 默认为false
  loading?: boolean
  // 是否开启简洁的数据结构响应, 默认为true
  reductDataFormat?: boolean
  // 是否开启接口错误信息展示,默认为true
  showErrorMessage?: boolean
  // 是否开启code不为0时的信息提示, 默认为true
  showCodeMessage?: boolean
  // 是否开启code为0时的信息提示, 默认为false
  showSuccessMessage?: boolean
  // 当前请求使用另外的用户token
  anotherToken?: string
}
