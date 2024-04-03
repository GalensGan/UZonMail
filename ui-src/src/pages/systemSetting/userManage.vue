<template>
  <q-table class="full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination"
    :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <CreateBtn @click="onNewUserClick" />
    </template>

    <template v-slot:top-right>
      <q-input borderless dense debounce="300" v-model="filter" placeholder="Search">
        <template v-slot:append>
          <q-icon name="search" />
        </template>
      </q-input>
    </template>

    <template v-slot:body-cell-userId="props">
      <q-td :props="props">
        <q-badge color="primary" :label="props.value" />
      </q-td>
      <ContextMenu :items="userManageContextItems" :value="props.row" />
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestPage, TTableFilterObject } from 'src/compositions/types'
import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'

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

const originalRows = [
  { id: 1, name: 'Frozen Yogurt', calories: 159, fat: 6.0, carbs: 24, protein: 4.0, sodium: 87, calcium: '14%', iron: '1%' },
  { id: 2, name: 'Ice cream sandwich', calories: 237, fat: 9.0, carbs: 37, protein: 4.3, sodium: 129, calcium: '8%', iron: '1%' },
  { id: 3, name: 'Eclair', calories: 262, fat: 16.0, carbs: 23, protein: 6.0, sodium: 337, calcium: '6%', iron: '7%' },
  { id: 4, name: 'Cupcake', calories: 305, fat: 3.7, carbs: 67, protein: 4.3, sodium: 413, calcium: '3%', iron: '8%' },
  { id: 5, name: 'Gingerbread', calories: 356, fat: 16.0, carbs: 49, protein: 3.9, sodium: 327, calcium: '7%', iron: '16%' },
  { id: 6, name: 'Jelly bean', calories: 375, fat: 0.0, carbs: 94, protein: 0.0, sodium: 50, calcium: '0%', iron: '0%' },
  { id: 7, name: 'Lollipop', calories: 392, fat: 0.2, carbs: 98, protein: 0, sodium: 38, calcium: '0%', iron: '2%' },
  { id: 8, name: 'Honeycomb', calories: 408, fat: 3.2, carbs: 87, protein: 6.5, sodium: 562, calcium: '0%', iron: '45%' },
  { id: 9, name: 'Donut', calories: 452, fat: 25.0, carbs: 51, protein: 4.9, sodium: 326, calcium: '2%', iron: '22%' },
  { id: 10, name: 'KitKat', calories: 518, fat: 26.0, carbs: 65, protein: 7, sodium: 54, calcium: '12%', iron: '6%' },
  { id: 11, name: 'Frozen Yogurt-1', calories: 159, fat: 6.0, carbs: 24, protein: 4.0, sodium: 87, calcium: '14%', iron: '1%' },
  { id: 12, name: 'Ice cream sandwich-1', calories: 237, fat: 9.0, carbs: 37, protein: 4.3, sodium: 129, calcium: '8%', iron: '1%' },
  { id: 13, name: 'Eclair-1', calories: 262, fat: 16.0, carbs: 23, protein: 6.0, sodium: 337, calcium: '6%', iron: '7%' },
  { id: 14, name: 'Cupcake-1', calories: 305, fat: 3.7, carbs: 67, protein: 4.3, sodium: 413, calcium: '3%', iron: '8%' },
  { id: 15, name: 'Gingerbread-1', calories: 356, fat: 16.0, carbs: 49, protein: 3.9, sodium: 327, calcium: '7%', iron: '16%' },
  { id: 16, name: 'Jelly bean-1', calories: 375, fat: 0.0, carbs: 94, protein: 0.0, sodium: 50, calcium: '0%', iron: '0%' },
  { id: 17, name: 'Lollipop-1', calories: 392, fat: 0.2, carbs: 98, protein: 0, sodium: 38, calcium: '0%', iron: '2%' },
  { id: 18, name: 'Honeycomb-1', calories: 408, fat: 3.2, carbs: 87, protein: 6.5, sodium: 562, calcium: '0%', iron: '45%' },
  { id: 19, name: 'Donut-1', calories: 452, fat: 25.0, carbs: 51, protein: 4.9, sodium: 326, calcium: '2%', iron: '22%' },
  { id: 20, name: 'KitKat-1', calories: 518, fat: 26.0, carbs: 65, protein: 7, sodium: 54, calcium: '12%', iron: '6%' },
  { id: 21, name: 'Frozen Yogurt-2', calories: 159, fat: 6.0, carbs: 24, protein: 4.0, sodium: 87, calcium: '14%', iron: '1%' },
  { id: 22, name: 'Ice cream sandwich-2', calories: 237, fat: 9.0, carbs: 37, protein: 4.3, sodium: 129, calcium: '8%', iron: '1%' },
  { id: 23, name: 'Eclair-2', calories: 262, fat: 16.0, carbs: 23, protein: 6.0, sodium: 337, calcium: '6%', iron: '7%' },
  { id: 24, name: 'Cupcake-2', calories: 305, fat: 3.7, carbs: 67, protein: 4.3, sodium: 413, calcium: '3%', iron: '8%' },
  { id: 25, name: 'Gingerbread-2', calories: 356, fat: 16.0, carbs: 49, protein: 3.9, sodium: 327, calcium: '7%', iron: '16%' },
  { id: 26, name: 'Jelly bean-2', calories: 375, fat: 0.0, carbs: 94, protein: 0.0, sodium: 50, calcium: '0%', iron: '0%' },
  { id: 27, name: 'Lollipop-2', calories: 392, fat: 0.2, carbs: 98, protein: 0, sodium: 38, calcium: '0%', iron: '2%' },
  { id: 28, name: 'Honeycomb-2', calories: 408, fat: 3.2, carbs: 87, protein: 6.5, sodium: 562, calcium: '0%', iron: '45%' },
  { id: 29, name: 'Donut-2', calories: 452, fat: 25.0, carbs: 51, protein: 4.9, sodium: 326, calcium: '2%', iron: '22%' },
  { id: 30, name: 'KitKat-2', calories: 518, fat: 26.0, carbs: 65, protein: 7, sodium: 54, calcium: '12%', iron: '6%' },
  { id: 31, name: 'Frozen Yogurt-3', calories: 159, fat: 6.0, carbs: 24, protein: 4.0, sodium: 87, calcium: '14%', iron: '1%' },
  { id: 32, name: 'Ice cream sandwich-3', calories: 237, fat: 9.0, carbs: 37, protein: 4.3, sodium: 129, calcium: '8%', iron: '1%' },
  { id: 33, name: 'Eclair-3', calories: 262, fat: 16.0, carbs: 23, protein: 6.0, sodium: 337, calcium: '6%', iron: '7%' },
  { id: 34, name: 'Cupcake-3', calories: 305, fat: 3.7, carbs: 67, protein: 4.3, sodium: 413, calcium: '3%', iron: '8%' },
  { id: 35, name: 'Gingerbread-3', calories: 356, fat: 16.0, carbs: 49, protein: 3.9, sodium: 327, calcium: '7%', iron: '16%' },
  { id: 36, name: 'Jelly bean-3', calories: 375, fat: 0.0, carbs: 94, protein: 0.0, sodium: 50, calcium: '0%', iron: '0%' },
  { id: 37, name: 'Lollipop-3', calories: 392, fat: 0.2, carbs: 98, protein: 0, sodium: 38, calcium: '0%', iron: '2%' },
  { id: 38, name: 'Honeycomb-3', calories: 408, fat: 3.2, carbs: 87, protein: 6.5, sodium: 562, calcium: '0%', iron: '45%' },
  { id: 39, name: 'Donut-3', calories: 452, fat: 25.0, carbs: 51, protein: 4.9, sodium: 326, calcium: '2%', iron: '22%' },
  { id: 40, name: 'KitKat-3', calories: 518, fat: 26.0, carbs: 65, protein: 7, sodium: 54, calcium: '12%', iron: '6%' }
]

const { pagination, rows, filter, onTableRequest, loading } = useQTable({
  getRowsNumberCount: async () => originalRows.length,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest: async (filterObj: TTableFilterObject, pagination: IQtableRequestPage) => {
    return originalRows.slice(pagination.skip, pagination.skip + pagination.limit)
  }
})

// 右键菜单
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'
import { IContextMenuItem } from 'src/components/contextMenu/types'
const userManageContextItems: IContextMenuItem[] = [
  {
    name: 'addUser',
    label: '新增',
    tooltip: '新增用户',
    onClick: () => { }
  },
  {
    name: 'deleteUser',
    label: '删除',
    tooltip: '删除用户',
    color: 'negative',
    onClick: () => { }
  }
]

// 新增用户
import { showDialog } from 'src/components/popupDialog/PopupDialog'
import { PopupDialogFieldType } from 'src/components/popupDialog/types'
async function onNewUserClick () {
  const dialogResult = await showDialog({
    dataSet: {
      userIds: ['1', '2', '3']
    },
    title: '新增用户',
    fields: [
      { name: 'test', label: 'test', type: PopupDialogFieldType.text },
      { name: 'test', label: 'test', type: PopupDialogFieldType.text }
    ]
  })
  console.log('onNewUserClick', dialogResult)
}
</script>

<style lang="scss" scoped></style>
