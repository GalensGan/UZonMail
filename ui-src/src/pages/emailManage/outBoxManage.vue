<template>
  <q-table class="full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
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
        {{ props.value }}
      </q-td>
      <ContextMenu :items="userManageContextItems" :value="props.row" />
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'
import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import { getFilteredUsersCount, getFilteredUsersData } from 'src/api/user'

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
async function onRequest (filterObj: TTableFilterObject, pagination: IQtableRequestParams) {
  const { data } = await getFilteredUsersData(filterObj.filter as string, pagination)
  return data
}

const { pagination, rows, filter, onTableRequest, loading } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})
</script>

<style lang="scss" scoped></style>
