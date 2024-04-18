<template>
  <div class="card-like full-height column q-pa-md scroll-y no-wrap">
    <q-input v-model="emailInfo.subject" label="主题" dense placeholder="请输入邮件主题" class="q-mb-sm">
      <template v-slot:before>
        <q-icon name="subject" color="primary" />
      </template>
    </q-input>

    <SelectEmailTemplate v-model="emailInfo.templates" class="q-mb-sm" />

    <SelectEmailData class="q-mb-sm" />

    <SelectEmailBox :emailBoxType="0" icon="directions_run" label="发件人" class="q-mb-sm" icon-color="secondary"
      placeholder="请选择发件箱 (必须)" />

    <SelectEmailBox :emailBoxType="1" icon="settings_accessibility" label="收件人" placeholder="请选择收件箱 (必须)"
      class="q-mb-sm" />

    <q-editor v-model="text" class="scroll-y column no-wrap q-pa-xs flex q-mb-sm"
      placeholder="在此处输入模板内容, 变量使用 {{ }} 号包裹, 例如 {{ variableName }}">
    </q-editor>

    <q-uploader class="full-width q-mb-sm" url="http://localhost:4444/upload" label="附件" multiple />
    <SelectEmailBox icon="follow_the_signs" label="抄送" />
  </div>
</template>

<script lang="ts" setup>
import SelectEmailTemplate from './components/SelectEmailTemplate.vue'
import SelectEmailBox from './components/SelectEmailBox.vue'
import SelectEmailData from './components/SelectEmailData.vue'

const emailInfo = ref({
  subject: '', // 主题
  templates: [], // 模板 id
  data: [], // 用户发件数据
  senders: [], // 发件人邮箱
  receivers: [], // 收件人邮箱
  ccs: [], // 抄送人邮箱
  body: '', // 邮件正文
  // 附件必须先上传，此处保存的是附件的Id
  attachmentIds: [] // 附件
})

const text = ref<string>('')
</script>

<style lang="scss" scoped></style>
