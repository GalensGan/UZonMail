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
        <q-img :src="getTemplateImage(props.row)" error-src="/icons/undraw_mailbox_re_dvds.svg"
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
import SearchInput from 'src/components/searchInput/SearchInput.vue'
import ContextMenu from 'src/components/contextMenu/ContextMenu.vue'

import CreateBtn from 'src/components/componentWrapper/buttons/CreateBtn.vue'
import { IEmailTemplate, deleteEmailTemplate } from 'src/api/emailTemplate'
import { IContextMenuItem } from 'src/components/contextMenu/types'
import { confirmOperation, notifySuccess } from 'src/utils/notify'

// 模板接口
import { useEmailTemplateTable } from './compositions'
const { pagination, rows, filter, onTableRequest, loading, deleteRowById, getTemplateImage, onPreviewThumbnail } = useEmailTemplateTable()

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
</script>

<style lang="scss" scoped>
:deep(.q-table__grid-content) {
  align-content: start;
}
</style>
