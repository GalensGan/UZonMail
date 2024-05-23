<template>
  <q-table class="full-height full-width" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination"
    dense :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <div class="row justify-start items-center">
        <q-btn dense icon="west" class="q-mr-sm" flat size="sm" @click="goBackToSendHistory">
          <q-tooltip>
            返回到历史发件
          </q-tooltip>
        </q-btn>
        <div class="text-subtitle1">发件明细</div>
      </div>
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-index="props">
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
  const { data } = await getSendingItemsCount(sendingGroupId.value, filterObj.filter)
  return data || 0
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getSendingItemsData(sendingGroupId.value, filterObj.filter, pagination)
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
import { subscribeOne } from 'src/signalR/signalR'
import { UzonMailClientMethods } from 'src/signalR/types'

const isEmailSending = ref(false)
onMounted(async () => {
  // 读取初始状态
})
// 注册开始发件状态
function onGroupStartSending () {
  isEmailSending.value = true
}
subscribeOne(UzonMailClientMethods.groupStartSending, onGroupStartSending)
// 注册结束发件状态
function onGroupEndSending () {
  isEmailSending.value = false
}
subscribeOne(UzonMailClientMethods.groupEndSending, onGroupEndSending)

// 注册单个邮件发送进度回调
function onSendingItemProgressChanged () {

}
subscribeOne(UzonMailClientMethods.sendingItemProgressChanged, onSendingItemProgressChanged)
// 注册所有邮件发送组进度回调
function onSendingGroupProgressChanged () {

}
subscribeOne(UzonMailClientMethods.sendingGroupProgressChanged, onSendingGroupProgressChanged)
// 注册邮件组开始发件回调，主要用于计划型的发件
// #endregion
</script>

<style lang="scss" scoped></style>
