/* eslint-disable @typescript-eslint/no-explicit-any */
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { IPopupDialogField, IPopupDialogParams, PopupDialogFieldType } from 'src/components/popupDialog/types'
import { IEmailGroupListItem } from '../components/types'

import { IOutbox, createOutbox, createOutboxes } from 'src/api/emailBox'

import { notifyError, notifySuccess } from 'src/utils/dialog'
import { useUserInfoStore } from 'src/stores/user'
import { aes } from 'src/utils/encrypt'
import { IExcelColumnMapper, readExcel, writeExcel } from 'src/utils/file'
import { getUsableProxies } from 'src/api/proxy'

function encryptPassword (smtpPasswordSecretKeys: string[], password: string) {
  return aes(smtpPasswordSecretKeys[0], smtpPasswordSecretKeys[1], password)
}

/**
 * 获取发件箱字段
 * @param smtpPasswordSecretKeys
 * @returns
 */
export async function getOutboxFields (smtpPasswordSecretKeys: string[]): Promise<IPopupDialogField[]> {
  // 获取所有的代理
  const { data: proxyOptions } = await getUsableProxies()
  proxyOptions.unshift({
    id: 0,
    name: '无',
    isActive: true,
    proxy: ''
  })
  return [
    {
      name: 'email',
      type: PopupDialogFieldType.email,
      label: 'smtp发件邮箱',
      value: '',
      required: true
    },
    {
      name: 'name',
      type: PopupDialogFieldType.text,
      label: '发件人名称',
      value: ''
    },
    {
      name: 'smtpHost',
      label: 'smtp地址',
      type: PopupDialogFieldType.text,
      value: '',
      required: true
    },
    {
      name: 'smtpPort',
      label: 'smtp端口',
      type: PopupDialogFieldType.number,
      value: 25,
      required: true
    },
    {
      name: 'userName',
      type: PopupDialogFieldType.text,
      label: 'smtp用户名',
      placeholder: '可为空，若为空，则使用发件邮箱作用用户名',
      value: ''
    },
    {
      name: 'password',
      label: 'smtp密码',
      type: PopupDialogFieldType.password,
      required: true,
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
      parser: async (value: any) => {
        const pwd = String(value)
        // 对密码进行加密
        return encryptPassword(smtpPasswordSecretKeys, pwd)
      },
      value: ''
    },
    {
      name: 'description',
      label: '描述'
    },
    {
      name: 'proxyId',
      label: '代理',
      type: PopupDialogFieldType.selectOne,
      value: 0,
      placeholder: '为空时使用系统设置',
      options: proxyOptions,
      optionLabel: 'name',
      optionValue: 'id',
      optionTooltip: 'description',
      mapOptions: true,
      emitValue: true
    },
    {
      name: 'replyToEmails',
      label: '回信收件人'
    },
    {
      name: 'enableSSL',
      label: '启用 SSL',
      type: PopupDialogFieldType.boolean,
      value: true,
      required: true
    }
  ]
}

function getOutboxExcelDataMapper (): IExcelColumnMapper[] {
  return [
    {
      headerName: 'smtp邮箱',
      fieldName: 'email',
      required: true
    },
    {
      headerName: '发件人名称',
      fieldName: 'name'
    },
    {
      headerName: 'smtp用户名',
      fieldName: 'userName'
    },
    {
      headerName: 'smtp密码',
      fieldName: 'password',
      required: true
    },
    {
      headerName: 'smtp地址',
      fieldName: 'smtpHost',
      required: true
    },
    {
      headerName: 'smtp端口',
      fieldName: 'smtpPort',
      required: true
    },
    {
      headerName: '描述',
      fieldName: 'description'
    },
    {
      headerName: '代理',
      fieldName: 'proxy'
    },
    {
      headerName: '回信收件人',
      fieldName: 'replyToEmails'
    },
    {
      headerName: '使用 SSL',
      fieldName: 'EnableSSL'
    }
  ]
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
export function UseHeaderFunction (emailGroup: Ref<IEmailGroupListItem>,
  addNewRow: (newRow: Record<string, any>) => void) {
  const userInfoStore = useUserInfoStore()

  // 新建发件箱
  async function onNewOutboxClick () {
    // 新增发件箱
    const popupParams: IPopupDialogParams = {
      title: `新增发件箱 / ${emailGroup.value.label}`,
      fields: await getOutboxFields(userInfoStore.smtpPasswordSecretKeys)
    }

    // 弹出对话框
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    const { ok, data } = await showDialog<IOutbox>(popupParams)
    if (!ok) return
    // 新建请求
    // 添加邮箱组
    data.emailGroupId = emailGroup.value.id
    const { data: outbox } = await createOutbox(data)
    // 保存到 rows 中
    addNewRow(outbox)

    notifySuccess('新增发件箱成功')
  }

  // 导出模板
  async function onExportOutboxTemplateClick () {
    const data: any[] = [
      {
        email: '填写发件邮箱(导入时，请删除该行数据)',
        name: '填写发件人名称(可选)',
        userName: '填写 smtp 用户名，若与邮箱一致，则设置不填写',
        password: '填写 smtp 密码',
        smtpHost: '填写 smtp 地址',
        smtpPort: 25,
        description: '描述(可选)',
        proxy: '格式为：http://username:password@domain:port(可选)',
        replyToEmails: '回信收件人(多个使用逗号分隔)'
      }
    ]
    await writeExcel(data, {
      fileName: '发件箱模板.xlsx',
      sheetName: '发件箱',
      mappers: getOutboxExcelDataMapper()
    })

    notifySuccess('模板下载成功，请在下载目录中查看')
  }

  // 导入发件箱
  async function onImportOutboxClick () {
    const data = await readExcel({
      sheetIndex: 0,
      selectSheet: true,
      mappers: getOutboxExcelDataMapper()
    })

    if (data.length === 0) {
      notifyError('未找到可导入的数据')
      return
    }

    // 对密码进行加密
    data.forEach(row => {
      row.password = encryptPassword(userInfoStore.smtpPasswordSecretKeys, row.password)
      row.emailGroupId = emailGroup.value.id
    })

    // 向服务器请求新增
    const { data: outboxes } = await createOutboxes(data as IOutbox[])
    outboxes.forEach(x => {
      addNewRow(x)
    })

    notifySuccess('导入成功')
  }

  return {
    onNewOutboxClick,
    onExportOutboxTemplateClick,
    onImportOutboxClick
  }
}
