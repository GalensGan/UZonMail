<template>
  <q-table class="full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
    hide-header grid :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <CreateBtn @click="onNewEmailTemplate" tooltip="新增邮件模板" />
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body="props">
      <q-tr :props="props">
        <q-td key="name" :props="props">
          {{ props.row.name }}
        </q-td>
        <q-td key="calories" :props="props">
          <q-badge color="green">
            {{ props.row.calories }}
          </q-badge>
        </q-td>
        <q-td key="fat" :props="props">
          <q-badge color="purple">
            {{ props.row.fat }}
          </q-badge>
        </q-td>
        <q-td key="carbs" :props="props">
          <q-badge color="orange">
            {{ props.row.carbs }}
          </q-badge>
        </q-td>
        <q-td key="protein" :props="props">
          <q-badge color="primary">
            {{ props.row.protein }}
          </q-badge>
        </q-td>
        <q-td key="sodium" :props="props">
          <q-badge color="teal">
            {{ props.row.sodium }}
          </q-badge>
        </q-td>
        <q-td key="calcium" :props="props">
          <q-badge color="accent">
            {{ props.row.calcium }}
          </q-badge>
        </q-td>
        <q-td key="iron" :props="props">
          <q-badge color="amber">
            {{ props.row.iron }}
          </q-badge>
        </q-td>
      </q-tr>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { QTableColumn } from 'quasar'
import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

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

// 打开模板编辑器
const router = useRouter()
async function onNewEmailTemplate () {
  // 新增或编辑
  router.push({
    name: 'templateEditor'
  })
}
</script>

<style lang="scss" scoped></style>
