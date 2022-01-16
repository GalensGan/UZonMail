import request from '@/utils/request'

// 新建预览
export function newPreview(
  subject,
  receivers,
  data,
  templateId,
  attachments,
  copyToEmails
) {
  return request({
    url: '/send/preview',
    method: 'post',
    data: {
      subject,
      receivers,
      data,
      templateId,
      attachments,
      copyToEmails
    }
  })
}

// 获取预览
export function getPreviewData(key) {
  return request({
    url: `/send/preview/${key}`,
    method: 'get'
  })
}

// 获取发件状态
export function getCurrentStatus() {
  return request({
    url: `/send/status`,
    method: 'get'
  })
}

// 新建发送，只有在没有发送过时，才调用
// 重新发送只需要调用startSending
export function newSendTask(
  senders,
  subject,
  receivers,
  data,
  templateId,
  attachments,
  copyToEmails
) {
  return request({
    url: '/send/task',
    method: 'post',
    data: {
      senders,
      subject,
      receivers,
      data,
      templateId,
      attachments,
      copyToEmails
    }
  })
}

// 开始发送
export function startSending(historyId) {
  return request({
    url: `/send/tasks/${historyId}`,
    method: 'post'
  })
}

// 获取发送进度信息
export function getSendingInfo() {
  return request({
    url: `/send/info`,
    method: 'get'
  })
}
