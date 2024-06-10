/* eslint-disable @typescript-eslint/no-explicit-any */
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { IPopupDialogParams, PopupDialogFieldType } from 'src/components/popupDialog/types'
import { IEmailGroupListItem } from '../components/types'

import { IInbox, createInbox, createInboxes } from 'src/api/emailBox'

import { notifyError, notifySuccess } from 'src/utils/dialog'
import { IExcelColumnMapper, readExcel, writeExcel } from 'src/utils/file'

export function getInboxFields () {
  return [
    {
      name: 'email',
      type: PopupDialogFieldType.email,
      label: '邮箱',
      value: '',
      required: true
    },
    {
      name: 'name',
      type: PopupDialogFieldType.text,
      label: '收件人名称'
    },
    {
      name: 'description',
      label: '描述'
    }
  ]
}

function getInboxExcelDataMapper (): IExcelColumnMapper[] {
  return [
    {
      headerName: '邮箱',
      fieldName: 'email',
      required: true
    },
    {
      headerName: '收件人名称',
      fieldName: 'name'
    },
    {
      headerName: '描述',
      fieldName: 'description'
    }
  ]
}

/**
 * 弹出新增收件箱弹窗
 * @param emailGroupLabel
 * @returns
 */
export async function showNewInboxDialog (emailGroupLabel: string) {
  // 新增发件箱
  const popupParams: IPopupDialogParams = {
    title: `新增收件箱 / ${emailGroupLabel}`,
    fields: getInboxFields()
  }

  // 弹出对话框
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  return await showDialog<IInbox>(popupParams)
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
export function UseHeaderFunction (emailGroup: Ref<IEmailGroupListItem>,
  addNewRow: (newRow: Record<string, any>) => void) {
  // 新建发件箱
  async function onNewInboxClick () {
    // 新增发件箱
    // 弹出对话框
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const { ok, data } = await showNewInboxDialog(emailGroup.value.label)
    if (!ok) return
    // 新建请求
    // 添加邮箱组
    data.emailGroupId = emailGroup.value.id
    const { data: outbox } = await createInbox(data)
    // 保存到 rows 中
    addNewRow(outbox)

    notifySuccess('新增发件箱成功')
  }

  // 导出模板
  async function onExportInboxTemplateClick () {
    const data: any[] = [
      {
        email: '邮箱(导入时，请删除该行数据)',
        description: ''
      }
    ]
    await writeExcel(data, {
      fileName: '收件箱模板.xlsx',
      sheetName: '收件箱',
      mappers: getInboxExcelDataMapper()
    })

    notifySuccess('模板下载成功，请在下载目录中查看')
  }

  // 导入发件箱
  async function onImportInboxClick () {
    const data = await readExcel({
      sheetIndex: 0,
      selectSheet: true,
      mappers: getInboxExcelDataMapper()
    })

    if (data.length === 0) {
      notifyError('未找到可导入的数据')
      return
    }

    // 添加组
    data.forEach(row => {
      row.emailGroupId = emailGroup.value.id
    })

    // 向服务器请求新增
    const { data: inboxes } = await createInboxes(data as IInbox[])
    inboxes.forEach(x => {
      addNewRow(x)
    })

    notifySuccess('导入成功')
  }

  return {
    onNewInboxClick,
    onExportInboxTemplateClick,
    onImportInboxClick
  }
}
