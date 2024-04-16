<template>
  <q-table class="full-height" :rows="rows" row-key="id" v-model:pagination="pagination" dense hide-header grid
    :loading="loading" :filter="filter" binary-state-sort @request="onTableRequest">
    <template v-slot:top-left>
      <CreateBtn @click="onNewEmailTemplate" tooltip="新增邮件模板" />
    </template>

    <template v-slot:top-right>
      <SearchInput v-model="filter" />
    </template>

    <template v-slot:item="props">
      <div class="q-card rounded-borders q-ma-sm" style="height: 180px; width: 300px">
        <q-img :src="getTemplateImage(props.row)" error-src="/public/icons/undraw_mailbox_re_dvds.svg"
          spinner-color="white" class="full-width full-height cursor-pointer" fit="cover" position="left top"
          @click="onPreviewThumbnail(props.row)">

          <template v-slot:loading>
            <q-spinner-gears color="white" />
          </template>

          <template v-slot:error>
            <div class="absolute-bottom row items-center justify-center">
              <div class="text-h6 q-mr-sm hover-underline" @click.self.stop="onEditTemplateClick(props.row)">
                {{ props.row.name }}
                <q-tooltip>
                  单击编辑模板
                </q-tooltip>
              </div>
              <div class="text-secondary">ID:{{ props.row.id }}</div>
            </div>
            <ContextMenu :items="contextItemsForError" :value="props.row"></ContextMenu>
          </template>

          <div class="absolute-bottom row items-center justify-center">
            <div class="text-h6 q-mr-sm hover-underline" @click.self.stop="onEditTemplateClick(props.row)">
              {{ props.row.name }}
              <q-tooltip>
                单击编辑模板
              </q-tooltip>
            </div>
            <div class="text-secondary">ID:{{ props.row.id }}</div>
          </div>

          <ContextMenu :items="templateContextMenuItems" :value="props.row"></ContextMenu>
        </q-img>
      </div>
    </template>
  </q-table>
</template>

<script lang="ts" setup>
import { useQTable } from 'src/compositions/qTableUtils'
import { IQtableRequestParams, TTableFilterObject } from 'src/compositions/types'
import SearchInput from 'src/components/searchInput/SearchInput.vue'
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'

import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import { IEmailTemplate, getEmailTemplatesCount, getEmailTemplatesData, deleteEmailTemplate } from 'src/api/emailTemplate'
import { IContextMenuItem } from 'src/components/contextMenu/types'
import { confirmOperation, notifySuccess } from 'src/utils/notify'

async function getRowsNumberCount (filterObj: TTableFilterObject) {
  const { data } = await getEmailTemplatesCount(filterObj.filter)
  return data
}
async function onRequest (filterObj: TTableFilterObject, pagination: IQtableRequestParams) {
  const { data } = await getEmailTemplatesData(filterObj.filter as string, pagination)
  return data
}

const { pagination, rows, filter, onTableRequest, loading, deleteRowById } = useQTable({
  getRowsNumberCount,
  onRequest
})

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function getTemplateImage (row: Record<string, any>) {
  if (!row) return '/public/icons/undraw_mailbox_re_dvds.svg'
  let baseUrl = process.env.BASE_URL as string
  baseUrl = baseUrl.replace('/api/v1', '')
  const url = baseUrl + '/' + row.thumbnail
  console.log(url)
  return url
}

// 打开模板编辑器
const router = useRouter()
async function onNewEmailTemplate () {
  // 新增或编辑
  router.push({
    name: 'templateEditor'
  })
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function onDeleteEmailTemplate (templateItem: Record<string, any>) {
  const templateData = templateItem as IEmailTemplate
  // 提示
  const confirm = await confirmOperation('删除确认', `确认删除模板: ${templateData.name} 吗？`)
  if (!confirm) return

  // 向服务器请求删除模板
  await deleteEmailTemplate(templateData.id as number)

  // 更新删除
  deleteRowById(templateData.id)

  notifySuccess('删除成功')
}
// eslint-disable-next-line @typescript-eslint/no-explicit-any
async function onEditTemplateClick (value: Record<string, any>) {
  router.push({
    name: 'templateEditor',
    query: {
      templateId: value.id,
      tagName: value.id
    }
  })
}
// 右键菜单
const templateContextMenuItems = ref<IContextMenuItem[]>([
  {
    name: 'preview',
    label: '预览',
    tooltip: '查看预览图',
    onClick: async () => {
      router.push({
        name: 'templateEditor'
      })
    }
  },
  {
    name: 'edit',
    label: '编辑',
    tooltip: '编辑当前模板',
    onClick: onEditTemplateClick
  },
  {
    name: 'delete',
    label: '删除',
    tooltip: '删除当前模板',
    color: 'negative',
    onClick: onDeleteEmailTemplate
  }
])
const contextItemsForError = computed(() => {
  return templateContextMenuItems.value.filter(item => item.name !== 'preview')
})

// 查看缩略图
import 'viewerjs/dist/viewer.css'
import { api as viewerApi } from 'v-viewer'
// eslint-disable-next-line @typescript-eslint/no-explicit-any
function onPreviewThumbnail (row: Record<string, any>) {
  const imageUrl = getTemplateImage(row)

  viewerApi({
    options: {
      title: false,
      transition: false,
      navbar: false
    },
    images: [imageUrl]
  })
}
</script>

<style lang="scss" scoped>
:deep(.q-table__grid-content) {
  align-content: start;
}
</style>
