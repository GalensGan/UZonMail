<template>
  <div class="full-height full-width row items-start">
    <EmailGroupList v-if="!isCollapseGroupList" v-model="emailGroupRef" class="q-card q-mr-sm"
      style="width: 160px;" />

    <q-table class="col full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
      :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
      <template v-slot:top-left>
        <div class="row justify-start q-gutter-sm">
          <CreateBtn tooltip="新增发件箱" @click="onNewOutboxClick" :disable="!isValidEmailGroup" />
          <ExportBtn tooltip="导出发件箱模板" @click="onExportOutboxTemplateClick" />
          <ImportBtn tooltip="导入发件箱" @click="onImportOutboxClick" :disable="!isValidEmailGroup" />
        </div>
      </template>

      <template v-slot:top-right>
        <SearchInput v-model="filter" />
      </template>

      <template v-slot:body-cell-userId="props">
        <q-td :props="props">
          {{ props.value }}
        </q-td>
        <ContextMenu :items="outboxContextMenuItems" :value="props.row" />
      </template>
    </q-table>

    <CollapseLeft v-model="isCollapseGroupList" :class="collapseLeftClass" />
  </div>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import SearchInput from 'src/components/searchInput/SearchInput.vue'
import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import ImportBtn from 'src/components/componentWrapper/buttons/ImportBtn.vue'
import ExportBtn from 'src/components/componentWrapper/buttons/ExportBtn.vue'
import EmailGroupList from './components/EmailGroupList.vue'
import CollapseLeft from 'src/components/collapseLeft/CollapseLeft.vue'

import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'
import { getFilteredUsersCount, getFilteredUsersData } from 'src/api/user'
import { IContextMenuItem } from 'src/components/contextMenu/types'
import { IEmailGroupListItem } from './components/types'
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { IDialogResult, IPopupDialogParams, PopupDialogFieldType } from 'src/components/popupDialog/types'

// 左侧分组开关
const isCollapseGroupList = ref(false)
const collapseLeftClass = computed(() => {
  return {
    'collapse-groups__open': !isCollapseGroupList.value,
    'collapse-groups__close': isCollapseGroupList.value
  }
})

// 菜单项
const emailGroupRef: Ref<IEmailGroupListItem> = ref({
  id: 0,
  name: 'all',
  label: '',
  order: 0
})
const isValidEmailGroup = computed(() => emailGroupRef.value.id)

const columns: QTableColumn[] = [
  {
    name: 'email',
    required: true,
    label: '邮箱',
    align: 'left',
    field: 'email',
    sortable: true
  },
  {
    name: 'smtpHost',
    required: true,
    label: 'smtp地址',
    align: 'left',
    field: 'smtpHost',
    sortable: true
  },
  {
    name: 'smtpPort',
    required: true,
    label: 'smtp端口',
    align: 'left',
    field: 'smtpPort',
    sortable: true
  },
  {
    name: 'password',
    required: true,
    label: 'smtp密码',
    align: 'left',
    field: 'password',
    sortable: true
  },
  {
    name: 'name',
    required: true,
    label: '别名',
    align: 'left',
    field: 'name',
    sortable: true
  }
]
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getFilteredUsersCount(filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IQtableRequestParams) {
  const { data } = await getFilteredUsersData(filterObj.filter as string, pagination)
  return data
}
const { pagination, rows, filter, onTableRequest, loading } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})

// #region 表头功能
// 新建发件箱
async function onNewOutboxClick () {
  // 新增发件箱
  const popupParams: IPopupDialogParams = {
    title: '新增发件箱',
    fields: [
      {
        name: 'email',
        label: '邮箱',
        type: PopupDialogFieldType.text,
        value: '',
        required: true
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
        type: PopupDialogFieldType.text,
        value: 25,
        required: true
      },
      {
        name: 'password',
        label: 'smtp密码',
        type: PopupDialogFieldType.text,
        required: true
      },
      {
        name: 'name',
        label: '别名',
        type: PopupDialogFieldType.text,
        required: true
      }
    ]
  }

  // 弹出对话框
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  await showDialog<IDialogResult<any>>(popupParams)
}

// 导出模板
async function onExportOutboxTemplateClick () {

}

// 导入发件箱
async function onImportOutboxClick () {

}
// #endregion

// #region 数据右键菜单

const outboxContextMenuItems: IContextMenuItem[] = [
  {
    name: 'delete',
    label: '删除',
    tooltip: '删除当前发件箱',
    onClick: async () => { }
  }
]
// #endregion

</script>

<style lang="scss" scoped>
.collapse-groups__open {
  position: absolute;
  top: 40%;
  left: 190px;
}

.collapse-groups__close {
  position: absolute;
  top: 40%;
  left: 24px;
}
</style>
