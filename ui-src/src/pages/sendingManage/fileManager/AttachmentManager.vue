<template>
  <q-table ref="dropZoneRef" class="full-height" :rows="rows" :columns="columns" row-key="id"
    v-model:pagination="pagination" dense :loading="loading" :filter="filter" binary-state-sort
    @request="onTableRequest">
    <template v-slot:top-left>
      <CreateBtn label="上传" icon="upload" tooltip="上传附件" @click="open" />
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:body-cell-index="props">
      <QTableIndex :props="props" />
      <ContextMenu :items="attachmentCtxMenuItems" />
    </template>

    <template v-slot:body-cell-userId="props">
      <q-td :props="props">
        {{ props.value }}
      </q-td>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { QTableColumn, format } from 'quasar'
import { useQTable, useQTableIndex } from 'src/compositions/qTableUtils'
import { IRequestPagination, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'

const { indexColumn, QTableIndex } = useQTableIndex()
const columns: QTableColumn[] = [
  indexColumn,
  {
    name: 'displayName',
    required: true,
    label: '文件名',
    align: 'left',
    field: v => v.fileName || v.displayName,
    sortable: true
  },
  {
    name: 'sha256',
    required: true,
    label: '哈希值',
    align: 'left',
    field: v => v.fileObject.sha256,
    format: v => {
      // 截取两端的 hash 值显示
      return v ? v.slice(0, 8) + '...' + v.slice(-8) : ''
    },
    sortable: false
  },
  {
    name: 'linkCount',
    required: true,
    label: '引用数',
    align: 'left',
    field: v => v.fileObject.linkCount,
    sortable: false
  },
  {
    name: 'size',
    required: true,
    label: '文件大小',
    align: 'left',
    field: v => v.fileObject.size,
    sortable: false,
    format: v => v ? format.humanStorageSize(v) : ''
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
  }
]
import { getFileUsagesCount, getFileUsagesData } from 'src/api/file'
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getFileUsagesCount(filterObj.filter)
  return data || 0
}
// eslint-disable-next-line @typescript-eslint/no-unused-vars
async function onRequest (filterObj: TTableFilterObject, pagination: IRequestPagination) {
  const { data } = await getFileUsagesData(filterObj.filter, pagination)
  return data
}

const { pagination, rows, filter, onTableRequest, loading, refreshTable } = useQTable({
  getRowsNumberCount,
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  onRequest
})

import { notifySuccess, showComponentDialog } from 'src/utils/dialog'
import FilesUploaderPopup from 'src/components/uploader/FilesUploaderPopup.vue'
import { useDropZone, useFileDialog } from '@vueuse/core'

// #region 上传文件
const { open, onChange } = useFileDialog({
  multiple: true,
  // accept: 'image/*', // Set to accept only image files
  directory: false // Select directories instead of files if set true
})

onChange(async (files) => {
  if (!files) return
  await uploadFiles(Array.from(files))
})

async function uploadFiles (files: File[]) {
  if (files.length === 0) return

  // 打开上传弹窗
  await showComponentDialog(FilesUploaderPopup, {
    files
  })

  //  更新当前界面
  refreshTable()
}
// #endregion

// #region 拖拽上传
const dropZoneRef = ref<HTMLDivElement>()
async function onDrop (files: File[] | null) {
  // called when files are dropped on zone
  console.log('onDrop:', files)
  if (!files) return

  await uploadFiles(files)
}
const { isOverDropZone } = useDropZone(dropZoneRef, {
  onDrop
  // specify the types of data to be received.
  // dataTypes: ['image/jpeg']
})
watch(isOverDropZone, () => {
  if (isOverDropZone.value) {
    notifySuccess('松开鼠标上传')
  }
})
// #endregion

// #region 右键菜单
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'
import { IContextMenuItem } from 'src/components/contextMenu/types'
const attachmentCtxMenuItems: IContextMenuItem[] = [
  {
    name: 'download',
    label: '下载',
    tooltip: '下载文件',
    // 当返回 false 时，右键菜单不会退出
    onClick: onDownloadAttachment
  },
  {
    name: 'rename',
    label: '重命名',
    tooltip: '重命名为有规律的名称，方便在发件时使用',
    // 当返回 false 时，右键菜单不会退出
    onClick: onDownloadAttachment
  },
  {
    name: 'delete',
    label: '删除',
    color: 'negative',
    tooltip: '删除该条记录,但不会真正删除文件',
    // 当返回 false 时，右键菜单不会退出
    onClick: onDownloadAttachment
  }
]
// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function onDownloadAttachment (row: Record<string, any>) {
  console.log('onDownloadAttachment:', row)
}
// #endregion
</script>

<style lang="scss" scoped></style>
