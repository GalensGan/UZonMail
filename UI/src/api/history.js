import request from '@/utils/request'

// 获取发件历史数量
export function getHistoriesCount(filter) {
  return request({
    url: '/histories/count',
    method: 'post',
    data: {
      filter
    }
  })
}

// 获取发件历史
export function getHistoriesList(filter, pagination) {
  return request({
    url: '/histories/list',
    method: 'post',
    data: {
      filter,
      pagination
    }
  })
}

// 获取某一个发件历史
export function getHistoryById(historyId) {
  return request({
    url: `/histories/${historyId}`,
    method: 'get'
  })
}

// 通过历史来获取所有的发送记录的数量
export function getSendItemsCountByHistoryId(historyId, filter) {
  return request({
    url: `/histories/${historyId}/items/count`,
    method: 'post',
    data: {
      filter
    }
  })
}

// 通过历史来获取所有的发送记录
export function getSendItemsListByHistoryId(historyId, filter, pagination) {
  return request({
    url: `/histories/${historyId}/items/list`,
    method: 'post',
    data: {
      filter,
      pagination
    }
  })
}

// 通过历史来获取所有的发送记录
export function getHistoryGroupSendResult(historyId) {
  return request({
    url: `/send/history/${historyId}/result`,
    method: 'get'
  })
}

// 删除历史记录
export function deleteHistoryGroup(historyId) {
  return request({
    url: `/histories/${historyId}`,
    method: 'delete'
  })
}
