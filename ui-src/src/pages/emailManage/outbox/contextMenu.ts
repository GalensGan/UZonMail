/* eslint-disable @typescript-eslint/no-explicit-any */
import { IOutbox, deleteOutboxById, updateOutbox } from 'src/api/emailBox'

import { IContextMenuItem } from 'src/components/contextMenu/types'
import { IPopupDialogParams } from 'src/components/popupDialog/types'
import { confirmOperation, notifySuccess } from 'src/utils/dialog'
import { getOutboxFields } from './headerFunctions'
import { useUserInfoStore } from 'src/stores/user'
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { deAes } from 'src/utils/encrypt'

/**
 * 获取smtp密码
 * @param IOutbox
 * @returns
 */
function getSmtpPassword (outbox: IOutbox, smtpPasswordSecretKeys: string[]) {
  if (outbox.decryptedPassword) return outbox.password
  return deAes(smtpPasswordSecretKeys[0], smtpPasswordSecretKeys[1], outbox.password)
}

export function useContextMenu (deleteRowById: (id?: number) => void) {
  const userInfoStore = useUserInfoStore()

  // 删除发件箱
  async function deleteOutbox (row: Record<string, any>) {
    const outbox = row as IOutbox
    // 提示是否删除
    const confirm = await confirmOperation('删除发件箱', `是否删除发件箱: ${outbox.email}？`)
    if (!confirm) return

    await deleteOutboxById(outbox.id as number)

    // 开始删除
    deleteRowById(outbox.id)

    notifySuccess('删除成功')
  }

  // 更新发件箱
  async function onUpdateOutbox (row: Record<string, any>) {
    const outbox = row as IOutbox

    const fields = await getOutboxFields(userInfoStore.smtpPasswordSecretKeys)
    // 修改默认值
    fields.forEach(field => {
      switch (field.name) {
        case 'email':
          field.value = outbox.email
          break
        case 'name':
          field.value = outbox.name
          break
        case 'userName':
          field.value = outbox.userName
          break
        case 'password':
          field.value = getSmtpPassword(outbox, userInfoStore.smtpPasswordSecretKeys)
          break
        case 'smtpHost':
          field.value = outbox.smtpHost
          break
        case 'smtpPort':
          field.value = outbox.smtpPort
          break
        case 'description':
          field.value = outbox.description
          break
        case 'proxyId':
          field.value = outbox.proxyId
          break
        default:
          break
      }
    })

    // 新增发件箱
    const popupParams: IPopupDialogParams = {
      title: `修改发件箱 / ${outbox.email}`,
      fields
    }

    // 弹出对话框
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const { ok, data } = await showDialog<IOutbox>(popupParams)
    if (!ok) return

    // 向服务器传递更新
    await updateOutbox(outbox.id as number, data)

    // 将参数更新到 outbox 中
    Object.assign(outbox, data, { decryptedPassword: false, showPassword: false })

    notifySuccess('更新成功')
  }

  const outboxContextMenuItems: IContextMenuItem[] = [
    {
      name: 'edit',
      label: '编辑',
      tooltip: '编辑当前发件箱',
      onClick: onUpdateOutbox
    },
    {
      name: 'delete',
      label: '删除',
      tooltip: '删除当前发件箱',
      color: 'negative',
      onClick: deleteOutbox
    }
  ]

  return { outboxContextMenuItems }
}
