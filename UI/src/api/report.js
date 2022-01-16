import request from '@/utils/request'

// 新建预览
export function getSuccessRate() {
  return request({
    url: '/report/success-rate',
    method: 'get'
  })
}

// 获取邮箱类型对应的数量
export function getInboxCountOfTyes() {
  return request({
    url: '/report/inbox-type-count',
    method: 'get'
  })
}
