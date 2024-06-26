<template>
  <q-list dense>
    <q-item class="plain-list__item text-primary bg-grey-12" v-ripple>
      <q-item-section avatar class="q-pr-none">
        <q-icon :name="header.icon" />
      </q-item-section>
      <q-item-section class="q-px-lg q-py-sm">{{ header.label }}</q-item-section>

      <ContextMenu v-if="!readonly" :items="headerContextMenuItems"></ContextMenu>
    </q-item>

    <q-separator />

    <q-item class="plain-list__item q-my-xs" v-for="item in sortedItems" :key="item.name" clickable v-ripple
      :active="item.active" active-class="text-secondary" @click="onItemClick(item)">
      <div class="row justify-between no-wrap items-center full-width">
        <q-icon v-if="item.icon" :name="item.icon || 'contact_mail'" size="sm" />
        <div v-if="item.label">{{ item.label }}
          <AsyncTooltip :tooltip="item.label" />
        </div>
        <div side v-if="item.side">{{ item.side }}</div>
      </div>

      <ContextMenu v-if="!readonly" :items="itemContextMenuItems" :value="item"></ContextMenu>
    </q-item>
  </q-list>
</template>

<script lang="ts" setup>
import { PropType } from 'vue'
import { IEmailGroupListItem, IFlatHeader } from './types'
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'
import AsyncTooltip from 'src/components/asyncTooltip/AsyncTooltip.vue'
import { IContextMenuItem } from 'src/components/contextMenu/types'

const modelValue = defineModel<IEmailGroupListItem>()

const props = defineProps({
  // 树形结构顶部的菜单
  // 通过 order 来控制显示位置
  extraItems: {
    type: Array as PropType<IEmailGroupListItem[]>,
    default: () => []
  },

  // 只读模式
  readonly: {
    type: Boolean,
    default: false
  },

  // 组类型
  // 1-发件
  // 2-收件
  groupType: {
    type: Number as PropType<1 | 2>,
    default: 1
  }
})
const header: ComputedRef<IFlatHeader> = computed(() => {
  if (props.groupType === 1) {
    return {
      label: '发件箱',
      icon: 'group'
    }
  }
  return {
    label: '收件箱',
    icon: 'group'
  }
})
const groupItems = ref<IEmailGroupListItem[]>([])
const sortedItems = computed(() => {
  const results = []
  if (props.extraItems.length > 0) {
    results.push(...props.extraItems)
  }
  results.push(...groupItems.value)
  results.sort((a, b) => a.order - b.order)
  return results
})

// 初始化获取组
import { getEmailGroups, createEmailCroup, IEmailGroup, updateEmailCroup, deleteEmailGroupById } from 'src/api/emailGroup'
import { IPopupDialogParams, PopupDialogFieldType } from 'src/components/popupDialog/types'
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { confirmOperation, notifySuccess } from 'src/utils/dialog'
onMounted(async () => {
  const { data: groups } = await getEmailGroups(props.groupType)
  groupItems.value = groups.map(x => ({ ...x, label: x.name, active: false, side: String(x.order) }))

  // 默认选中第一个
  if (sortedItems.value.length > 0) {
    activeGroup(sortedItems.value[0])
  }
})
function activeGroup (group: IEmailGroupListItem) {
  sortedItems.value.forEach(x => { x.active = false })
  group.active = true
  modelValue.value = group
}

async function onItemClick (item: IEmailGroupListItem) {
  activeGroup(item)
}

// #region 右键菜单相关
// 新增邮箱组
async function onCreateEmailGroup () {
  const popupParams: IPopupDialogParams = {
    title: '新增邮箱组',
    fields: [
      {
        name: 'name',
        label: '组名',
        value: '',
        type: PopupDialogFieldType.text,
        required: true
      },
      {
        name: 'description',
        label: '描述',
        value: '',
        type: PopupDialogFieldType.textarea
      },
      {
        name: 'order',
        label: '序号',
        value: groupItems.value.length + 1,
        type: PopupDialogFieldType.text
      }
    ],
    oneColumn: true
  }

  // 弹出对话框
  const result = await showDialog<IEmailGroup>(popupParams)
  if (!result.ok) return

  // 添加默认的 icon
  const { data: group } = await createEmailCroup({
    icon: 'group',
    ...result.data,
    type: props.groupType
  })

  groupItems.value.push({
    ...group,
    label: group.name,
    active: false,
    side: String(group.order)
  })

  // 设置新组为当前组
  activeGroup(groupItems.value[groupItems.value.length - 1])

  // 新增组
  notifySuccess('新增成功')
}
// 分类 header 右键
const headerContextMenuItems: IContextMenuItem[] = [
  {
    name: 'add',
    label: '新增',
    tooltip: '新增邮箱组',
    onClick: onCreateEmailGroup
  }
]

/**
 * 具体分类上的右键菜单
 */

// 修改分组
// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function modifyGroup (emailGroup: Record<string, any>) {
  const typedEmailGroup = emailGroup as IEmailGroupListItem
  const popupParams: IPopupDialogParams = {
    title: '修改邮箱组',
    fields: [
      {
        name: 'name',
        label: '组名',
        value: typedEmailGroup.label,
        type: PopupDialogFieldType.text,
        required: true
      },
      {
        name: 'description',
        label: '描述',
        value: typedEmailGroup.description,
        type: PopupDialogFieldType.textarea
      },
      {
        name: 'order',
        label: '序号',
        value: typedEmailGroup.order || typedEmailGroup.side,
        validate: async (value: number) => {
          const numValue = Number(value)
          if (isNaN(numValue)) {
            return {
              ok: false,
              message: '请输入数字'
            }
          }
          return { ok: true }
        },
        type: PopupDialogFieldType.text
      }
    ],
    oneColumn: true
  }

  // 弹出对话框
  const result = await showDialog<IEmailGroup>(popupParams)
  if (!result.ok) return

  // 指定 id
  result.data.id = typedEmailGroup.id
  await updateEmailCroup(result.data)

  // 更新数据
  typedEmailGroup.label = result.data.name
  typedEmailGroup.order = result.data.order
  typedEmailGroup.side = String(result.data.order)

  // 新增组
  notifySuccess('新增成功')
}
// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function deleteGroup (emailGroup: Record<string, any>) {
  // 进行确认
  const confirm = await confirmOperation('确认删除', `即将删除组【${emailGroup.label}】和其中所有的邮箱，是否继续？`)
  if (!confirm) return

  // 向服务器请求删除
  await deleteEmailGroupById(emailGroup.id)

  // 从当前列表中清除组
  const groupIndex = groupItems.value.findIndex(x => x.id === emailGroup.id)
  groupItems.value.splice(groupIndex, 1)

  // 切换到临近的组
  const newIndex = Math.max(0, groupIndex - 1)
  if (groupItems.value.length > 0) {
    groupItems.value[newIndex].active = true
    modelValue.value = groupItems.value[newIndex]
  } else {
    modelValue.value = {
      name: 'all',
      label: '',
      order: 0
    }
  }

  notifySuccess(`删除组【${emailGroup.label}】成功`)
}
const itemContextMenuItems: IContextMenuItem[] = [
  ...headerContextMenuItems,
  {
    name: 'modify',
    label: '修改',
    tooltip: '修改当前分组',
    onClick: modifyGroup
  },
  {
    name: 'delete',
    label: '删除',
    color: 'negative',
    tooltip: '删除当前分组',
    onClick: deleteGroup
  }
]
// #endregion
</script>

<style lang="scss" scoped>
.plain-list__item {
  :deep(.q-item__section--avatar) {
    min-width: auto !important;
  }
}
</style>
