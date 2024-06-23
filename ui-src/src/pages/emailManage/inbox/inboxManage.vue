<template>
  <div class="full-height full-width row items-start">
    <EmailGroupList v-show="!isCollapseGroupList" v-model="emailGroupRef" :groupType="2" class="q-card q-mr-sm"
      style="width: 160px;" />

    <q-table class="col full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
      :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
      <template v-slot:top-left>
        <div class="row justify-start q-gutter-sm">
          <CreateBtn tooltip="新增收件箱" @click="onNewInboxClick" :disable="!isValidEmailGroup"
            tooltip-when-disabled="请先添加组" />
          <ExportBtn label="模板" tooltip="导出收件箱模板" @click="onExportInboxTemplateClick" />
          <ImportBtn tooltip="导入收件箱" @click="onImportInboxClick" :disable="!isValidEmailGroup"
            tooltip-when-disabled="请先添加组" />
        </div>
      </template>

      <template v-slot:top-right>
        <SearchInput v-model="filter" />
      </template>

      <template v-slot:body-cell-index="props">
        <QTableIndex :props="props" />
      </template>

      <template v-slot:body-cell-email="props">
        <q-td :props="props">
          {{ props.value }}
        </q-td>
        <ContextMenu :items="inboxContextMenuItems" :value="props.row" />
      </template>
    </q-table>

    <CollapseLeft v-model="isCollapseGroupList" :class="collapseLeftClass" />
  </div>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import { formatDateStr } from 'src/utils/format'

import SearchInput from 'src/components/searchInput/SearchInput.vue'
import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import ImportBtn from 'src/components/componentWrapper/buttons/ImportBtn.vue'
import ExportBtn from 'src/components/componentWrapper/buttons/ExportBtn.vue'
import EmailGroupList from '../components/EmailGroupList.vue'
import CollapseLeft from 'src/components/collapseLeft/CollapseLeft.vue'
import ContextMenu from 'components/contextMenu/ContextMenu.vue'

import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import { getInboxesCount, getInboxesData } from 'src/api/emailBox'
import { IEmailGroupListItem } from '../components/types'

// 左侧分组开关
const isCollapseGroupList = ref(false)
const collapseLeftClass = computed(() => {
  return {
    'collapse-groups__open': !isCollapseGroupList.value,
    'collapse-groups__close': isCollapseGroupList.value
  }
})

const { indexColumn, QTableIndex } = useQTableIndex()
// 菜单项
const emailGroupRef: Ref<IEmailGroupListItem> = ref({
  id: 0,
  name: 'all',
  label: '',
  order: 0
})
const isValidEmailGroup = computed(() => emailGroupRef.value.id)

const columns: QTableColumn[] = [
  indexColumn,
  {
    name: 'email',
    label: '邮箱',
    align: 'left',
    field: 'email',
    sortable: true
  },
  {
    name: 'name',
    label: '名称(收件人姓名)',
    align: 'left',
    field: 'name',
    sortable: true
  },
  {
    name: 'description',
    label: '描述',
    align: 'left',
    field: 'description',
    sortable: true
  },
  {
    name: 'minInboxCooldownHours',
    label: '最小发件间隔(h)',
    align: 'left',
    field: 'minInboxCooldownHours',
    sortable: true
  },
  {
    name: 'lastSuccessDeliveryDate',
    label: '最近发件日期',
    align: 'left',
    field: 'lastSuccessDeliveryDate',
    format: v => formatDateStr(v),
    sortable: true
  }
]
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getInboxesCount(emailGroupRef.value.id, filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getInboxesData(emailGroupRef.value.id, filterObj.filter, pagination)
  return data
}
const { pagination, rows, filter, onTableRequest, loading, refreshTable, addNewRow, deleteRowById } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})
watch(emailGroupRef, () => {
  // 组切换时，触发更新
  refreshTable()
})

// #region 表头功能
import { UseHeaderFunction } from './headerFunctions'
const { onNewInboxClick, onExportInboxTemplateClick, onImportInboxClick } = UseHeaderFunction(emailGroupRef, addNewRow)
// #endregion

// #region 数据右键菜单
import { useContextMenu } from './contextMenu'
const { inboxContextMenuItems } = useContextMenu(deleteRowById)
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
