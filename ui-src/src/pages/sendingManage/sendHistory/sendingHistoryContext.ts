/* eslint-disable @typescript-eslint/no-explicit-any */
import { IContextMenuItem } from 'src/components/contextMenu/types'
import { showComponentDialog } from 'src/components/popupDialog/PopupDialog'
import SendDetailDialog from './SendDetailDialog.vue'

/**
 * 添加右键菜单
 */
export function useContextMenu () {
  // 打开发件明细
  async function openSendDetailDialog (data: Record<string, any>) {
    await showComponentDialog(SendDetailDialog, {
      sendingGroupId: data.id
    })
  }

  // 暂停发件
  async function pauseSending (data: Record<string, any>) {
    console.log(data)
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
      onClick: pauseSending
    },
    {
      name: 'start',
      label: '开始',
      tooltip: '重新开始发件',
      onClick: pauseSending
    },
    {
      name: 'stop',
      label: '中止',
      tooltip: '中止发件',
      onClick: pauseSending
    },
    {
      name: 'viewRawData',
      label: '详细',
      tooltip: '查看该发件组的原始数据',
      onClick: openSendDetailDialog
    }
  ]

  return {
    sendingHistoryContextItems
  }
}
