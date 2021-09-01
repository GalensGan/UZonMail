import request from "@/utils/request";

// 获取发件历史
export function getHistories() {
  return request({
    url: "/histories",
    method: "get"
  });
}

// 获取发件历史
export function getHistory(historyId) {
  return request({
    url: `/histories/${historyId}`,
    method: "get"
  });
}

// 通过历史来获取所有的发送记录
export function getSendItemsByHistoryId(historyId) {
  return request({
    url: `/histories/${historyId}/items`,
    method: "get"
  });
}

// 通过历史来获取所有的发送记录
export function getHistoryGroupSendResult(historyId) {
  return request({
    url: `/send/history/${historyId}/result`,
    method: "get"
  });
}

// 删除历史记录
export function deleteHistoryGroup(historyId) {
  return request({
    url: `/histories/${historyId}`,
    method: "delete"
  });
}
