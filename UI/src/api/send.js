import request from "@/utils/request";

// 添加模板
export function newSendTask(subject, receivers, data, templateId) {
  return request({
    url: "/send/task",
    method: "post",
    data: {
      subject,
      receivers,
      data,
      templateId
    }
  });
}

// 获取预览
export function getPreviewData(key) {
  return request({
    url: `/send/preview/${key}`,
    method: "get"
  });
}

// 获取发件状态
export function getCurrentStatus() {
  return request({
    url: `/send/status`,
    method: "get"
  });
}

// 开始发送
export function startSending() {
  return request({
    url: `/send`,
    method: "post"
  });
}

// 获取发送进度信息
export function getSendingInfo() {
  return request({
    url: `/send/info`,
    method: "get"
  });
}
