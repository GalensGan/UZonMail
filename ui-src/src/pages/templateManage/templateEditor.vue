<template>
  <q-editor class="full-height column no-wrap q-pa-xs" v-model="editorValue" :definitions="editorDefinitions"
    placeholder="在此处输入模板内容, 变量使用 {{  }} 号包裹, 例如 {{ variableName }}" :toolbar="editorToolbar">
    <template v-slot:templateName>
      <q-input borderless standout dense v-model="templateName" placeholder="输入模板名称">
        <template v-slot:prepend>
          <q-icon name="article" size="xs" />
        </template>
      </q-input>
    </template>
  </q-editor>
</template>

<script lang="ts" setup>
const editorValue = ref('')
const templateId = ref(0)
const templateName = ref('')

import { getEmailTemplateById, upsertEmailTemplate } from 'src/api/emailTemplate'
import { notifyError, notifySuccess } from 'src/utils/notify'
// 从服务器拉取内容
const route = useRoute()
onMounted(async () => {
  if (!route.query.templateId) return
  templateId.value = Number(route.query.templateId)
  // 获取数据
  const { data: templateData } = await getEmailTemplateById(templateId.value)
  templateName.value = templateData.name
  editorValue.value = templateData.content
})

const editorDefinitions = {
  save: {
    tip: '保存模板',
    icon: 'save',
    label: '保存',
    handler: saveTemplate
  }
}
const editorToolbar = ref([
  ['templateName'],
  ['save'],
  ['bold', 'italic', 'strike', 'underline'],
  ['undo', 'redo'],
  ['viewsource']
])
// 保存模板
import domToImage from 'dom-to-image'
import { uploadToStaticFile } from 'src/api/file'

async function saveTemplate () {
  if (!templateName.value) {
    notifyError('请输入模板名称')
    return
  }

  // 生成缩略图并上传到服务器
  const blob = await new Promise((resolve) => {
    const node = document.querySelector('.q-editor__content')
    domToImage.toBlob(node)
      .then(function (blob: Blob) {
        resolve(blob)
      })
  })

  const { data: url } = await uploadToStaticFile('template-thumbnails', `${templateName.value}.png`, blob as Blob)
  // 保存模板
  const templateData = {
    id: templateId.value,
    name: templateName.value,
    content: editorValue.value,
    thumbnail: url
  }

  const { data: { id } } = await upsertEmailTemplate(templateData)
  templateId.value = id as number
  notifySuccess('保存成功')
}
</script>

<style lang="scss" scoped>
:deep(.q-editor__toolbar) {
  align-items: center;
}
</style>
