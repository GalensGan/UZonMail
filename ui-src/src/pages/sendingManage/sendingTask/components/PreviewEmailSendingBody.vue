<template>
  <q-dialog ref='dialogRef' @hide="onDialogHide">
    <q-card class='column justify-start q-pa-sm width-600 height-400'>
      <div class="text-subtitle1 text-primary">
        主题：{{ emailSubject }}
      </div>

      <div class="text-secondary">
        收件人: {{ currentInbox }}
      </div>

      <q-separator class="q-mb-sm" />

      <div class="q-pb-md col scroll-y" v-html="emailBody"></div>

      <q-pagination v-if="pagesCount > 1" class="self-center" v-model="currentPage" :max="pagesCount" :max-pages="6"
        boundary-numbers />
    </q-card>
  </q-dialog>
</template>

<script lang='ts' setup>
// 预览邮件发件正文

/**
 * warning: 该组件一个弹窗的示例，不可直接使用
 * 参考：http://www.quasarchs.com/quasar-plugins/dialog#composition-api-variant
 */

import { useDialogPluginComponent } from 'quasar'
defineEmits([
  // 必需；需要指定一些事件
  // （组件将通过useDialogPluginComponent()发出）
  ...useDialogPluginComponent.emits
])
const { dialogRef, onDialogHide /* , onDialogOK, onDialogCancel */ } = useDialogPluginComponent()

import { IEmailCreateInfo } from 'src/api/emailSending'
const props = defineProps({
  emailCreateInfo: {
    type: Object as PropType<IEmailCreateInfo>,
    required: true
  }
})

// 分页
const pagesCount = ref(0)
const currentPage = ref(1)

// #region 预览邮件发件正文逻辑
// 若用户数据中有收件箱，则要与收件箱中的数据进行合并
let inboxes = [...props.emailCreateInfo.inboxes]
const userDataInboxes = props.emailCreateInfo.data.map((item) => item.inbox)
  .filter(x => x)
if (userDataInboxes.length > 0) {
  inboxes.push(...userDataInboxes)
}
// 去重
inboxes = Array.from(new Set(inboxes))
pagesCount.value = inboxes.length

import { useInstanceRequestCache } from 'src/api/base/httpCache'
import { getEmailTemplateById } from 'src/api/emailTemplate'
const cacheKey = useInstanceRequestCache()
// 主题
function getSubjects () {
  // 通过分号，换行符进行分隔
  // 先将所有的 ; 和 ； 替换为 \n
  const regex = /;|；/gm
  const subject = props.emailCreateInfo.subjects.replace(regex, '\n')
  return subject.split('\n').filter(x => x)
}
const subjects = getSubjects()
// eslint-disable-next-line @typescript-eslint/no-explicit-any
function applyVariablesToTemplate (userData: Record<string, any>, templateContent: string) {
  if (!templateContent || !userData) return templateContent

  // 应用变量
  for (const key of Object.keys(userData)) {
    const regex = new RegExp(`{{\\s*${key}\\s*}}`, 'gm')
    templateContent = templateContent.replace(regex, userData[key])
  }
  return templateContent
}

async function getEmailBody (inbox: string, inboxIndex: number) {
  // 正文优先级：用户数据/正文 > 用户数据/模板 > 界面/正文 > 界面/模板
  const userData = props.emailCreateInfo.data.find((item) => item.inbox === inbox)
  if (userData) {
    // 查找模板
    const templateIndex = inboxIndex % props.emailCreateInfo.templates.length
    const template = props.emailCreateInfo.templates[templateIndex]
    // 从服务器拉取模板数据
    const { data } = await getEmailTemplateById(template.id, cacheKey)

    // 对模板应用变量
    return applyVariablesToTemplate(userData, data.content)
  }

  // 说明没有变量，直接从界面正文中获取
  if (props.emailCreateInfo.body) return props.emailCreateInfo.body

  // 判断是有模板数据,从模板中读取，但是没有变量
  if (props.emailCreateInfo.templates.length > 0) {
    // 查找模板
    const templateIndex = inboxIndex % props.emailCreateInfo.templates.length
    const template = props.emailCreateInfo.templates[templateIndex]
    const { data } = await getEmailTemplateById(template.id, cacheKey)

    // 返回模板数据
    return data.content
  }

  return ''
}

const currentInbox = ref('')
const emailBody = ref('')
const emailSubject = ref('')
watch(currentPage, async () => {
  const index = currentPage.value - 1
  const inbox = inboxes[index].email as string
  currentInbox.value = inbox
  const subject = subjects[index % subjects.length]
  emailSubject.value = subject
  const body = await getEmailBody(inbox, index)
  emailBody.value = body
}, {
  immediate: true
})
// #endregion
</script>

<style lang='scss' scoped></style>
