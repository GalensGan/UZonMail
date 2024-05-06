import { IProxy } from 'src/api/proxy'
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { IPopupDialogField, IPopupDialogParams, PopupDialogFieldType } from 'src/components/popupDialog/types'
import { useUserInfoStore } from 'src/stores/user'

export function getCommonProxyFields (): IPopupDialogField[] {
  return [
    {
      name: 'name',
      type: PopupDialogFieldType.text,
      label: '名称',
      placeholder: '代理的唯一标识，需要保证唯一',
      value: '',
      required: true
    },
    {
      name: 'proxy',
      type: PopupDialogFieldType.text,
      label: '代理地址',
      placeholder: '格式：username:password@host 或 host',
      value: '',
      required: true
    },
    {
      name: 'matchRegex',
      label: '匹配规则',
      type: PopupDialogFieldType.number,
      placeholder: '使用正则表达式进行匹配',
      value: '.*'
    },
    {
      name: 'priority',
      label: '优先级',
      type: PopupDialogFieldType.number,
      placeholder: '数字越大优先级越高',
      value: 0
    },
    {
      name: 'description',
      label: '描述'
    }
  ]
}

/**
 * 顶部功能区
 * @returns
 */
export function useHeaderFunctions () {
  const userInfoStore = useUserInfoStore()
  async function onCreateProxy () {
    const fields = getCommonProxyFields()
    // 若是管理员，则添加共享字段
    if (userInfoStore.isAdmin) {
      fields.push({
        name: 'isShared',
        type: PopupDialogFieldType.boolean,
        label: '是否共享',
        value: false
      })
    }

    // 打开弹窗
    // 新增发件箱
    const popupParams: IPopupDialogParams = {
      title: '新增代理',
      fields
    }

    // 弹出对话框
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    await showDialog<IProxy>(popupParams)
  }

  return { onCreateProxy }
}
