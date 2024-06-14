<template>
  <q-table class="full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
    :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <CreateBtn @click="onNewUserClick" />
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-userId="props">
      <q-td :props="props">
        {{ props.value }}
      </q-td>
      <ContextMenu :items="userManageContextItems" :value="props.row" />
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import { useQTable } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import { notifyError, notifySuccess, confirmOperation } from 'src/utils/dialog'
import { getFilteredUsersCount, checkUserId, createUser, getFilteredUsersData, getDefaultPassword, resetUserPassword } from 'src/api/user'

const columns: QTableColumn[] = [
  {
    name: 'userId',
    required: true,
    label: '用户名',
    align: 'left',
    field: 'userId',
    sortable: true
  },
  {
    name: 'createDate',
    required: false,
    label: '注册日期',
    align: 'left',
    field: 'createDate',
    format: (val: string) => {
      return val ? new Date(val).toLocaleString() : ''
    },
    sortable: true
  }
]
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getFilteredUsersCount(filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getFilteredUsersData(filterObj.filter as string, pagination)
  return data
}

const { pagination, rows, filter, onTableRequest, loading, addNewRow } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})

// 右键菜单
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'
import { IContextMenuItem } from 'src/components/contextMenu/types'

// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function onResetUserPassword (userInfoInfo: Record<string, any>) {
  // 获取默认密码
  const { data: defaultPassword } = await getDefaultPassword()

  const confirm = await confirmOperation('重置密码', `密码即将重置为 ${defaultPassword}, 是否继续?`)
  if (!confirm) return false

  // 开始重置
  await resetUserPassword(userInfoInfo.userId)

  notifySuccess('重置密码成功')
}
const userManageContextItems: IContextMenuItem[] = [
  {
    name: 'addUser',
    label: '新增',
    tooltip: '新增用户',
    onClick: onNewUserClick
  },
  {
    name: 'resetPassword',
    label: '重置密码',
    tooltip: '重置用户密码',
    onClick: onResetUserPassword
  },
  {
    name: 'deleteUser',
    label: '删除',
    tooltip: '删除用户',
    color: 'negative',
    onClick: async () => {
      notifyError('暂不支持删除用户')
    }
  }
]

// 新增用户
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { PopupDialogFieldType } from 'src/components/popupDialog/types'
async function onNewUserClick () {
  const dialogResult = await showDialog({
    title: '新增用户',
    fields: [
      {
        name: 'userId',
        label: '用户名',
        type: PopupDialogFieldType.text,
        required: true,
        placeholder: '请输入用户名,请仅用英文字母',
        validate: async (value) => {
          return {
            ok: value && value.length >= 3,
            message: '用户名必须大小等于 3 个字符'
          }
        }
      },
      {
        name: 'password',
        label: '初始密码',
        type: PopupDialogFieldType.text,
        required: true,
        placeholder: '请输入初始密码',
        validate: async (value) => {
          return {
            ok: value && value.length >= 6,
            message: '密码必须大小等于 6 个字符'
          }
        }
      }
    ],
    validate: async (fieldsModel) => {
      console.log('validate', fieldsModel.userId)
      // 验证用户名是否重复
      const { data } = await checkUserId(fieldsModel.userId)
      return {
        ok: data,
        message: `用户名：${fieldsModel.userId}已存在`
      }
    }
  })
  console.log('onNewUserClick', dialogResult)
  if (!dialogResult.ok) return

  // 新增用户
  const { data: modelValue } = dialogResult
  const { data: newUser } = await createUser(modelValue.userId as string, modelValue.password as string)
  notifySuccess('新增用户成功')
  addNewRow(newUser)
}
</script>

<style lang="scss" scoped></style>
