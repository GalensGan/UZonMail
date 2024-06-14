<template>
  <q-table class="full-height full-width" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination"
    dense :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <div class="row justify-start items-center q-gutter-sm">
        <q-btn dense icon="west" class="q-mr-sm" flat size="sm" @click="goBackToSendHistory">
          <q-tooltip>
            返回到历史发件
          </q-tooltip>
        </q-btn>
        <div class="text-subtitle1">发件明细</div>

        <LinearProgress v-if="showProgressBar" :value="sendingGroupProgressValue" :width="160">
        </LinearProgress>
      </div>
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-index="props">
      <QTableIndex :props="props" />

      <ContextMenu :items="sendDetailContextItems" :value="props.row"></ContextMenu>
    </template>

    <template v-slot:body-cell-status="props">
      <q-td :props="props">
        <q-circular-progress v-if="props.value === 1" indeterminate size="sm" :thickness="0.4" color="secondary"
          track-color="grey-3" center-color="primary" />
        <StatusChip :status="props.value">
          <AsyncTooltip v-if="props.value === SendingItemStatus[SendingItemStatus.Failed]"
            :tooltip="props.row.sendResult"></AsyncTooltip>
        </StatusChip>
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
// 定义 props
const vueProps = defineProps({
  sendingGroupId: {
    type: Number,
    required: false
  }
})

const sendingGroupId = ref(vueProps.sendingGroupId)

import { QTableColumn } from 'quasar'
import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

import { getSendingItemsCount, getSendingItemsData, SendingItemStatus } from 'src/api/sendingItem'

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
  {
    name: 'inboxes',
    required: true,
    label: '收件箱',
    align: 'left',
    field: 'inboxes',
    sortable: true,
    format: v => {
      return v.map((e: { email: string }) => e.email).join(';')
    }
  },
  {
    name: 'status',
    required: true,
    label: '状态',
    align: 'left',
    field: 'status',
    sortable: true,
    format: v => SendingItemStatus[v]
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

// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getSendingItemsCount(sendingGroupId.value as number, filterObj.filter)
  return data || 0
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getSendingItemsData(sendingGroupId.value as number, filterObj.filter, pagination)
  return data || []
}

const { pagination, rows, filter, onTableRequest, loading, refreshTable } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})

import { removeHistory } from 'src/layouts/components/tags/routeHistories'
import { IRouteHistory } from 'src/layouts/components/tags/types'
const router = useRouter()
function goBackToSendHistory () {
  removeHistory(router, route as unknown as IRouteHistory, '/send-management/history')
}

// 从路由中获取id
const route = useRoute()
onMounted(async () => {
  if (!route.query.sendingGroupId) return
  sendingGroupId.value = Number(route.query.sendingGroupId)
  // 触发更新
  refreshTable()
})

// #region 发件进度相关
import LinearProgress from 'src/components/Progress/LinearProgress.vue'
import { subscribeOne } from 'src/signalR/signalR'
import { ISendingGroupProgressArg, ISendingItemStatusChangedArg, UzonMailClientMethods, SendingGroupProgressType } from 'src/signalR/types'
import { getSendingGroupRunningInfo, SendingGroupStatus } from 'src/api/sendingGroup'
const sendingGroupProgressValue = ref(-1)
const showProgressBar = computed(() => sendingGroupProgressValue.value > 0 && sendingGroupProgressValue.value < 1)
onMounted(async () => {
  // 读取初始状态
  const { data } = await getSendingGroupRunningInfo(sendingGroupId.value as number)
  if (data.status !== SendingGroupStatus.Sending) return
  sendingGroupProgressValue.value = data.sentCount / data.totalCount
})
// 注册单个邮件发送进度回调
function onSendingItemStatusChanged (arg: ISendingItemStatusChangedArg) {
  // 更新单个进度
  const row = rows.value.find(r => r.id === arg.sendingItemId)
  if (!row) return

  // 更新参数
  Object.assign(row, arg)
}
subscribeOne(UzonMailClientMethods.sendingItemStatusChanged, onSendingItemStatusChanged)
// 注册由件组的发送进度
import { notifySuccess } from 'src/utils/dialog'
function onSendingGroupProgressChanged (arg: ISendingGroupProgressArg) {
  // 更新总进度
  if (arg.sendingGroupId !== sendingGroupId.value) return
  if (arg.progressType === SendingGroupProgressType.end) {
    if (arg.sendingGroupId !== sendingGroupId.value) return
    sendingGroupProgressValue.value = -1

    notifySuccess(`邮件组 ${arg.sendingGroupId} 发送完成`)
    return
  }
  sendingGroupProgressValue.value = arg.current / arg.total
}
subscribeOne(UzonMailClientMethods.sendingGroupProgressChanged, onSendingGroupProgressChanged)
// #endregion

// #region 发送状态
import StatusChip from 'src/components/statusChip/StatusChip.vue'
import AsyncTooltip from 'src/components/asyncTooltip/AsyncTooltip.vue'
// #endregion

/**
 * 右键菜单
 */
import { useContextMenu } from './sendDetailContext'
const { sendDetailContextItems, ContextMenu } = useContextMenu()
</script>

<style lang="scss" scoped></style>
