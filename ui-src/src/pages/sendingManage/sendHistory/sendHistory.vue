<template>
  <q-table class="full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
    :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <div class="text-subtile1">历史发件</div>
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-index="props">
      <QTableIndex :props="props" />
      <ContextMenu :items="sendingHistoryContextItems" :value="props.row" />
    </template>

    <template v-slot:body-cell-subjects="props">
      <q-td :props="props">
        {{ props.value }}
      </q-td>
    </template>

    <template v-slot:body-cell-status="props">
      <q-td :props="props">
        <q-chip v-if="props.value !== 'Sending'" dense square :label="props.value"
          :color="getStatusColor(props.row.status)" />
        <LinearProgress class="full-width" v-else :value="props.row.progress" :width="60"></LinearProgress>
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import LinearProgress from 'src/components/Progress/LinearProgress.vue'

import { QTableColumn } from 'quasar'
import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

import { getSendingGroupsCount, getEmailTemplatesData, SendingGroupStatus, SendingGroupType } from 'src/api/sendingGroup'

const { indexColumn, QTableIndex } = useQTableIndex()
const columns: QTableColumn[] = [
  indexColumn,
  {
    name: 'id',
    label: 'ID',
    align: 'left',
    field: 'id',
    sortable: true
  },
  {
    name: 'subjects',
    required: true,
    label: '主题',
    align: 'left',
    field: 'subjects',
    sortable: true,
    format: v => v.replace('\n', ';')
  },
  {
    name: 'sendingType',
    required: true,
    label: '类型',
    align: 'left',
    field: 'sendingType',
    sortable: true,
    format: v => SendingGroupType[v]
  },
  {
    name: 'templatesCount',
    required: true,
    label: '模板数',
    align: 'left',
    field: 'templatesCount'
  },
  {
    name: 'outboxesCount',
    required: true,
    label: '发件箱数',
    align: 'left',
    field: 'outboxesCount'
  },
  {
    name: 'totalCount',
    required: true,
    label: '收件箱数',
    align: 'left',
    field: 'totalCount',
    sortable: true
  },
  {
    name: 'successCount',
    required: true,
    label: '成功数',
    align: 'left',
    field: 'successCount',
    sortable: true
  },
  {
    name: 'createDate',
    required: false,
    label: '创建日期',
    align: 'left',
    field: 'createDate',
    format: (val: string) => {
      return val ? new Date(val).toLocaleString() : ''
    },
    sortable: true
  }, {
    name: 'status',
    required: true,
    label: '状态',
    align: 'center',
    field: 'status',
    sortable: true,
    format: v => SendingGroupStatus[v]
  }
]

// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getSendingGroupsCount(filterObj.filter)
  return data || 0
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getEmailTemplatesData(filterObj.filter, pagination)
  return data || []
}

const { pagination, rows, filter, onTableRequest, loading } = useQTable({
  sortBy: 'id',
  descending: true,
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})

// 右键菜单
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'
import { useContextMenu } from './sendingHistoryContext'
const { sendingHistoryContextItems } = useContextMenu()

/**
 * 进度与状态显示
 */

// 获取状态颜色
function getStatusColor (status: number): string {
  switch (status) {
    case SendingGroupStatus.Cancel:
      return 'negative'
    case SendingGroupStatus.Pause:
      return 'warning'
    case SendingGroupStatus.Scheduled:
    case SendingGroupStatus.Finish:
    case SendingGroupStatus.Created:
      return 'primary'
    default:
      return 'accent'
  }
}
// 注册进度获取回调
import { subscribeOne } from 'src/signalR/signalR'
import { ISendingGroupProgressArg, UzonMailClientMethods } from 'src/signalR/types'
// 进度变化
function onSendingGroupProgressChanged (arg: ISendingGroupProgressArg) {
  const row = rows.value.find(r => r.id === arg.sendingGroupId)
  if (!row) return

  row.successCount = arg.successCount
  // 更新进度
  row.progress = arg.current * 1.0 / arg.total
}
subscribeOne(UzonMailClientMethods.sendingGroupProgressChanged, onSendingGroupProgressChanged)
</script>

<style lang="scss" scoped></style>
