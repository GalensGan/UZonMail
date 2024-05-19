<template>
  <q-table class="full-height full-width" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination"
    dense :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-id="props">
      <QTableIndex :props="props" />
    </template>

    <template v-slot:body-cell-userId="props">
      <q-td :props="props">
        {{ props.value }}
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
// 定义 props
const vueProps = defineProps({
  sendingGroupId: {
    type: Number,
    required: true
  }
})

import { QTableColumn } from 'quasar'
import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

const { indexColumn, QTableIndex } = useQTableIndex()
const columns: QTableColumn[] = [
  indexColumn,
  {
    name: 'subject',
    required: true,
    label: '主题',
    align: 'left',
    field: 'subject',
    sortable: true
  },
  {
    name: 'fromEmail',
    required: true,
    label: '发件箱',
    align: 'left',
    field: 'fromEmail',
    sortable: true
  },
  // {
  //   name: 'inboxes',
  //   required: true,
  //   label: '收件箱',
  //   align: 'left',
  //   field: 'inboxes',
  //   sortable: true
  // },
  {
    name: 'status',
    required: true,
    label: '状态',
    align: 'left',
    field: 'status',
    sortable: true
  },
  {
    name: 'sendResult',
    required: true,
    label: '发送结果',
    align: 'left',
    field: 'sendResult',
    sortable: true
  },
  {
    name: 'sendDate',
    required: false,
    label: '发送日期',
    align: 'left',
    field: 'sendDate',
    format: (val: string) => {
      return val ? new Date(val).toLocaleString() : ''
    },
    sortable: true
  }
]

import { getSendingItemsCount, getSendingItemsData } from 'src/api/sendingItem'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getSendingItemsCount(vueProps.sendingGroupId, filterObj.filter)
  return data || 0
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getSendingItemsData(vueProps.sendingGroupId, filterObj.filter, pagination)
  return data || []
}

const { pagination, rows, filter, onTableRequest, loading } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})
</script>

<style lang="scss" scoped></style>
