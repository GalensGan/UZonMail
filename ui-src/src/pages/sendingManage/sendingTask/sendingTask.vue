<template>
  <div class="card-like column q-pa-md scroll-y no-wrap">
    <q-input v-model="emailInfo.subject" autogrow label="主题" dense
      placeholder="请输入邮件主题(若需要随机主题，多个主题之间请使用分号 ; 进行分隔或者单独一行)" class="email-subject q-mb-sm" style="max-height:200px">
      <template v-slot:before>
        <q-icon name="subject" color="primary" />
      </template>
    </q-input>

    <SelectEmailTemplate v-model="emailInfo.templates" class="q-mb-sm" />

    <SelectEmailData v-model="emailInfo.data" class="q-mb-sm" />

    <SelectEmailBox v-model="emailInfo.outboxes" :emailBoxType="0" icon="directions_run" label="发件人" class="q-mb-sm"
      icon-color="secondary" placeholder="请选择发件箱 (必须)" />

    <SelectEmailBox v-model="emailInfo.inboxes" :emailBoxType="1" icon="settings_accessibility" label="收件人"
      placeholder="请选择收件箱 (必须)" class="q-mb-sm" />

    <SelectEmailBox v-model="emailInfo.ccBoxes" :emailBoxType="1" icon="settings_accessibility" label="抄送人"
      placeholder="请选择抄送人 (可选)" class="q-mb-sm" />

    <q-editor v-model="emailInfo.body" :definitions="editorDefinitions" :toolbar="editorToolbar"
      class="column no-wrap q-pa-xs flex q-ma-sm" style="max-height: 300px;"
      placeholder="在此处输入模板内容, 变量使用 {{ }} 号包裹, 例如 {{ variableName }}">
    </q-editor>

    <ObjectUploader v-model="emailInfo.attachments" v-model:need-upload="needUpload" label="附件" class="q-mx-sm q-mt-sm"
      style="width:auto" multiple />

    <div class="row justify-end items-center q-ma-sm q-mt-lg">
      <CommonBtn label="预览" color="accent" icon="view_carousel" tooltip="预览发件正文" @click="onPreviewClick" />
      <CommonBtn label="定时发送" class="q-ml-sm" color="primary" icon="schedule" tooltip="定时发件"
        @click="onScheduleSendClick" />
      <OkBtn label="发送" class="q-ml-sm" icon="alternate_email" tooltip="立即发件" @click="onSendNowClick" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import SelectEmailTemplate from './components/SelectEmailTemplate.vue'
import SelectEmailBox from './components/SelectEmailBox.vue'
import SelectEmailData from './components/SelectEmailData.vue'
import ObjectUploader from 'components/uploader/ObjectUploader.vue'
import { IEmailCreateInfo, useBottomFunctions } from './bottomFunctions'

const emailInfo: Ref<IEmailCreateInfo> = ref({
  subject: '', // 主题
  templates: [], // 模板 id
  data: [], // 用户发件数据
  outboxes: [], // 发件人邮箱
  inboxes: [], // 收件人邮箱
  ccBoxes: [], // 抄送人邮箱
  body: '', // 邮件正文
  // 附件必须先上传，此处保存的是附件的Id
  attachments: [] // 附件
})

// 编辑器配置
import { useWysiwygEditor } from 'pages/templateManage/compositions'
const { editorDefinitions, editorToolbar } = useWysiwygEditor()

// 底部功能按钮
const { OkBtn, CommonBtn, onPreviewClick, onScheduleSendClick, onSendNowClick } = useBottomFunctions(emailInfo)
</script>

<style lang="scss" scoped>
.email-subject :deep(textarea) {
  max-height: 120px;
  overflow-y: auto;
}
</style>
