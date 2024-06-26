<template>
  <div class="full-height full-width row items-start">
    <EmailGroupList v-show="!isCollapseGroupList" v-model="emailGroupRef" class="q-card q-mr-sm"
      style="width: 160px;" />

    <q-table class="col full-height" :rows="rows" :columns="columns" row-key="id" v-model:pagination="pagination" dense
      :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
      <template v-slot:top-left>
        <div class="row justify-start q-gutter-sm">
          <CreateBtn tooltip="新增发件箱" @click="onNewOutboxClick" :disable="!isValidEmailGroup" />
          <ExportBtn label="模板" tooltip="导出发件箱模板" @click="onExportOutboxTemplateClick" />
          <ImportBtn tooltip="导入发件箱" @click="onImportOutboxClick" :disable="!isValidEmailGroup" />
        </div>
      </template>

      <template v-slot:top-right>
        <SearchInput v-model="filter" />
      </template>

      <template v-slot:body-cell-id="props">
        <q-td :props="props">
          {{ props.rowIndex + 1 }}
        </q-td>
      </template>

      <template v-slot:body-cell-email="props">
        <q-td :props="props">
          {{ props.value }}
        </q-td>
        <ContextMenu :items="outboxContextMenuItems" :value="props.row" />
      </template>

      <template v-slot:body-cell-password="props">
        <q-td class="cursor-pointer" :props="props" @click="togglePasswordViewMode(props.row)">
          {{ getPasswordValue(props.row) }}
        </q-td>
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
import EmailGroupList from '../components/EmailGroupList.vue'
import CollapseLeft from 'src/components/collapseLeft/CollapseLeft.vue'
import ContextMenu from 'components/contextMenu/ContextMenu.vue'

import { useQTable } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import { getBoxesCount, getBoxesData, IOutbox } from 'src/api/emailBox'
import { IEmailGroupListItem } from '../components/types'

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
import { IProxy, getUsableProxies } from 'src/api/proxy'
const usableProxies: Ref<IProxy[]> = ref([])
onMounted(async () => {
  const { data: proxies } = await getUsableProxies()
  usableProxies.value = proxies
})
const columns: QTableColumn[] = [
  {
    name: 'id',
    required: true,
    label: '序号',
    align: 'left',
    field: 'id'
  },
  {
    name: 'email',
    required: true,
    label: '邮箱',
    align: 'left',
    field: 'email',
    sortable: true
  },
  {
    name: 'name',
    label: '名称(发件人姓名)',
    align: 'left',
    field: 'name',
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
    name: 'description',
    required: true,
    label: '描述',
    align: 'left',
    field: 'description',
    sortable: true
  },
  {
    name: 'proxyId',
    required: true,
    label: '代理',
    align: 'left',
    field: 'proxyId',
    sortable: true,
    format: (val: number) => {
      const proxy = usableProxies.value.find(p => p.id === val)
      return proxy?.name ?? '无'
    }
  }
]
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getBoxesCount(emailGroupRef.value.id, 0, filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getBoxesData<IOutbox>(emailGroupRef.value.id, 0, filterObj.filter, pagination)
  return data
}
const { pagination, rows, filter, onTableRequest, loading, refreshTable, addNewRow, deleteRowById } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest,
  preventRequestWhenMounted: true
})
watch(emailGroupRef, () => {
  // 组切换时，触发更新
  refreshTable()
})

function getPasswordValue (data: IOutbox) {
  if (data.showPassword) return data.password
  return '******'
}
import { deAes } from 'src/utils/encrypt'
import { useUserInfoStore } from 'src/stores/user'
const userInfoStore = useUserInfoStore()
async function togglePasswordViewMode (data: IOutbox) {
  // console.log('togglePasswordViewMode', data)
  // 若是显示密码，但没有解密，则先解密
  if (!data.decryptedPassword) {
    // 进行解密
    const plainPwd = deAes(userInfoStore.smtpPasswordSecretKeys[0], userInfoStore.smtpPasswordSecretKeys[1], data.password)
    data.password = plainPwd || '密钥变动,解密失败。请重新输入 smtp 密码'
    data.decryptedPassword = true
  }

  data.showPassword = !data.showPassword
}

// #region 表头功能
import { UseHeaderFunction } from './headerFunctions'
const { onNewOutboxClick, onExportOutboxTemplateClick, onImportOutboxClick } = UseHeaderFunction(emailGroupRef, addNewRow)
// #endregion

// #region 数据右键菜单
import { useContextMenu } from './contextMenu'
const { outboxContextMenuItems } = useContextMenu(deleteRowById)
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
