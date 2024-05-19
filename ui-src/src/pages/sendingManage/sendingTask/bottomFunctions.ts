/* eslint-disable @typescript-eslint/no-explicit-any */
// 底部按钮
import OkBtn from 'src/components/componentWrapper/buttons/OkBtn.vue'
import CommonBtn from 'src/components/componentWrapper/buttons/CommonBtn.vue'
import { notifyError, notifySuccess } from 'src/utils/notify'
import { IEmailCreateInfo, sendEmailNow } from 'src/api/emailSending'
import { useUserInfoStore } from 'src/stores/user'

import { showComponentDialog } from 'src/components/popupDialog/PopupDialog'
import PreviewEmailSendingBody from './components/PreviewEmailSendingBody.vue'
import SendingProgress from '../sendingProgress/SendingProgress.vue'

/**
 * 使用底部功能定义
 * @param emailInfo
 * @returns
 */
export function useBottomFunctions (emailInfo: Ref<IEmailCreateInfo>) {
  // 数据验证
  const needUpload = ref(false)
  const userInfoStore = useUserInfoStore()

  function validateParams () {
    console.log('email info:', emailInfo.value)
    if (!emailInfo.value.subject) {
      notifyError('请填写邮件主题')
      return false
    }

    if (!emailInfo.value.data.length) {
      if (!emailInfo.value.templates.length && !emailInfo.value.body) {
        notifyError('邮件模板和正文必须有一个不为空')
        return false
      }

      if (!emailInfo.value.outboxes.length) {
        notifyError('请选择发件人邮箱')
        return false
      }

      if (!emailInfo.value.inboxes.length) {
        notifyError('请选择收件人邮箱')
        return false
      }
    }

    if (needUpload.value) {
      notifyError('请单击附件上传按钮上传附件')
      return false
    }
    return true
  }
  async function onSendNowClick () {
    // if (!validateParams()) return
    console.log('email info:', emailInfo.value)

    // 向服务器推送数据
    await sendEmailNow(Object.assign({ smtpPasswordSecretKeys: userInfoStore.smtpPasswordSecretKeys }, emailInfo.value))

    await showComponentDialog(SendingProgress, {
      title: '发送进度'
    })

    // 将数据传到后台发送
    notifySuccess('开始发送...')
  }

  // 定时发送
  async function onScheduleSendClick () {
  }

  // 预览
  async function onPreviewClick () {
    if (!validateParams()) return

    // 预览功能在本机实现
    // 1. 正文优先级：用户数据/正文 > 用户数据/模板 > 界面/正文 > 界面/模板
    // 2. 变量参数只能从用户数据中提取
    // 3. 主题优先级: 用户数据/主题 > 界面/主题
    await showComponentDialog(PreviewEmailSendingBody, {
      emailCreateInfo: emailInfo.value
    })
  }

  return {
    OkBtn,
    CommonBtn,
    needUpload,
    onSendNowClick,
    onScheduleSendClick,
    onPreviewClick
  }
}
