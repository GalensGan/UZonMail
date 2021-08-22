import request from "@/utils/request";

export function getUserSettings() {
  return request({
    url: "/setting",
    method: "get"
  });
}

export function updateSendInterval(min, max) {
  return request({
    url: "/setting/send-interval",
    method: "put",
    data: {
      sendInterval: {
        min,
        max
      }
    }
  });
}

export function updateIsAutoResend(isAutoResend) {
  return request({
    url: "/setting/auto-resend",
    method: "put",
    data: {
      isAutoResend
    }
  });
}
