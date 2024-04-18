<template>
  <q-dialog ref='dialogRef' :persistent='true'>
    <q-card class='column no-wrap q-pa-sm zero-height' style="min-width: 500px; min-height: 400px;">
      <div class="col row items-start">
        <EmailGroupList v-model="emailGroupRef" readonly :groupType="groupType" class="q-mr-sm" style="width: 160px;" />

        <q-table class="col full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination"
          dense :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest" selection="multiple"
          v-model:selected="selected">
          <template v-slot:top-left>
            <div class="row justify-start q-gutter-sm">
              <CreateBtn tooltip="新增收件箱" @click="onNewTempInboxClick" v-if="isSelectedItemActive" />
            </div>
          </template>

          <template v-slot:top-right>
            <SearchInput v-model="filter" />
          </template>

          <template v-slot:body-cell-id="props">
            <QTableIndex :props="props" />
          </template>
        </q-table>
      </div>

      <div class="row justify-end items-center q-mt-md">
        <CancelBtn class="q-mr-sm" @click="onDialogCancel" />
        <OkBtn @click="onDialogOK" />
      </div>
    </q-card>
  </q-dialog>
</template>

<script lang='ts' setup>
// props 定义
const props = defineProps({
  emailBoxType: {
    // 0-发件箱 1-收件箱
    type: Number as PropType<0 | 1>,
    default: 0
  }
})
const { emailBoxType } = toRefs(props)
const groupType = computed(() => {
  return emailBoxType.value + 1 as 1 | 2
})

/**
 * warning: 该组件一个弹窗的示例，不可直接使用
 * 参考：http://www.quasarchs.com/quasar-plugins/dialog#composition-api-variant
 */

import { QTableColumn, useDialogPluginComponent } from 'quasar'
defineEmits([
  // 必需；需要指定一些事件
  // （组件将通过useDialogPluginComponent()发出）
  ...useDialogPluginComponent.emits
])
const { dialogRef, onDialogOK, onDialogCancel } = useDialogPluginComponent()

// 分类列表
import EmailGroupList from 'pages/emailManage/components/EmailGroupList.vue'
import { IEmailGroupListItem } from 'src/pages/emailManage/components/types'
const emailGroupRef: Ref<IEmailGroupListItem> = ref({
  name: 'selected',
  order: 0,
  label: '已选中'
})
const isSelectedItemActive = computed(() => {
  return emailGroupRef.value.name === 'selected'
})

// 表格定义与数据请求
import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
const { indexColumn, QTableIndex } = useQTableIndex()
const columns: QTableColumn[] = [
  indexColumn,
  {
    name: 'email',
    required: true,
    label: '邮箱',
    align: 'left',
    field: 'email',
    sortable: true
  },
  {
    name: 'description',
    required: true,
    label: '描述',
    align: 'left',
    field: 'description',
    sortable: true
  }
]
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'
import { IOutbox, getBoxesCount, getBoxesData } from 'src/api/emailBox'
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getBoxesCount(emailGroupRef.value.id, props.emailBoxType, filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IQtableRequestParams) {
  const { data } = await getBoxesData<IOutbox>(emailGroupRef.value.id, props.emailBoxType, filterObj.filter, pagination)
  return data
}
const { pagination, rows, filter, onTableRequest, loading, refreshTable, addNewRow } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})
watch(emailGroupRef, () => {
  // 组切换时，触发更新
  refreshTable()
})

// 选择结果
const selected = ref([])

// 新增收件箱
function onNewTempInboxClick () {
  addNewRow({})
}

// 底部确认
import CancelBtn from 'src/components/componentWrapper/buttons/CancelBtn.vue'
import OkBtn from 'src/components/componentWrapper/buttons/OkBtn.vue'
</script>

<style lang='scss' scoped></style>
