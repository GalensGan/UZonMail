/* eslint-disable @typescript-eslint/no-explicit-any */
import { httpClient } from 'src/api//base/httpClient'

export interface IEmailCreateInfo {
  subjects: string, // 主题
  templates: Record<string, any>[], // 模板 id
  data: Record<string, any>[], // 用户发件数据
  outboxes: Record<string, any>[], // 发件人邮箱
  inboxes: Record<string, any>[], // 收件人邮箱
  ccBoxes: Record<string, any>[], // 抄送人邮箱
  body: string, // 邮件正文
  // 附件必须先上传，此处保存的是附件的Id
  attachments: Record<string, any>[], // 附件
  smtpPasswordSecretKeys?: string[], // 发件人邮箱密码密钥, 发件时，需要由用户上传到服务器
}

/**
 * 立即发件
 * @param userId
 * @param type
 * @returns
 */
export function sendEmailNow (sendingGroup: IEmailCreateInfo) {
  return httpClient.post<boolean>('/email-sending/now', {
    data: sendingGroup
  })
}
