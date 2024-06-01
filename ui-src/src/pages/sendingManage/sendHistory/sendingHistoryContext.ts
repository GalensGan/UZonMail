/* eslint-disable @typescript-eslint/no-explicit-any */
import { IContextMenuItem } from 'src/components/contextMenu/types'
// import { showComponentDialog } from 'src/components/popupDialog/PopupDialog'
// import SendDetailDialog from './SendDetailDialog.vue'
import { SendingGroupStatus } from 'src/api/sendingGroup'
import { pauseSending, restartSending, cancelSending } from 'src/api/emailSending'
import { confirmOperation, notifySuccess } from 'src/utils/notify'

/**
 * 添加右键菜单
 */
export function useContextMenu () {
  const router = useRouter()
  // 打开发件明细
  async function openSendDetailDialog (data: Record<string, any>) {
    // 跳转到发件明细页面
    router.push({
      name: 'SendDetailTable',
      query: {
        sendingGroupId: data.id,
        tagName: data.id
      }
    })
  }

  // 暂停发件
  function canPauseSending (data: Record<string, any>): boolean {
    return data.status === SendingGroupStatus.Sending
  }
  async function onPauseSending (data: Record<string, any>) {
    // 进行确认
    const confirm = await confirmOperation('暂停确认', '确认暂停发件吗？')
    if (!confirm) return

    // 暂停发件
    // 向服务器请求暂停
    await pauseSending(data.id)

    data.status = SendingGroupStatus.Pause
    notifySuccess('暂停成功')
  }

  function canRestart (data: Record<string, any>): boolean {
    return data.status === SendingGroupStatus.Pause
  }
  async function onRestartSending (data: Record<string, any>) {
    // 进行确认
    const confirm = await confirmOperation('发送确认', '确认重新开始发件吗？')
    if (!confirm) return

    await restartSending(data.id)
    data.status = SendingGroupStatus.Sending

    notifySuccess('已重新发送')
  }

  // 是否可以取消发件
  // 非完成的发件组都可以取消
  function canCancel (data: Record<string, any>): boolean {
    return data.status !== SendingGroupStatus.Finish && data.status !== SendingGroupStatus.Cancel
  }
  async function onCancelSending (data: Record<string, any>) {
    // 进行确认
    const confirm = await confirmOperation('取消确认', '确认取消发件吗，取消后将不可重新开始，是否继续？')
    if (!confirm) return

    await cancelSending(data.id)
    data.status = SendingGroupStatus.Cancel

    notifySuccess('取消成功')
  }

  const sendingHistoryContextItems: IContextMenuItem[] = [
    {
      name: 'detail',
      label: '明细',
      tooltip: '查看发件明细',
      onClick: openSendDetailDialog
    },
    {
      name: 'pause',
      label: '暂停',
      tooltip: '暂停发件',
      onClick: onPauseSending,
      vif: canPauseSending
    },
    {
      name: 'start',
      label: '开始',
      tooltip: '重新开始发件',
      vif: canRestart,
      onClick: onRestartSending
    },
    {
      name: 'cancelSchedule',
      label: '取消',
      tooltip: '取消计划发件',
      color: 'negative',
      vif: canCancel,
      onClick: onCancelSending
    }
    // {
    //   name: 'viewRawData',
    //   label: '详细',
    //   tooltip: '查看该发件组的原始数据',
    //   onClick: openSendDetailDialog
    // }
  ]

  return {
    sendingHistoryContextItems
  }
}
