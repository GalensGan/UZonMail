<template>
  <q-field v-model="fieldModelValue" tag="div" label="数据" dense @focus="isActive = true" @blur="isActive = false">
    <template v-slot:before>
      <q-icon name="analytics" color="primary" />
    </template>

    <template v-slot:control>
      <div class="full-width no-outline">
        <div class="text-grey-7"> {{ fieldText }}</div>
      </div>
    </template>

    <template v-slot:append>
      <div class="row justify-end">
        <q-btn v-if="fieldModelValue" round dense flat icon="close" color="negative"
          @click.prevent="onRemoveSelectedFile">
          <q-tooltip>
            删除当前数据
          </q-tooltip>
        </q-btn>

        <q-btn v-if="isActive" round dense flat icon="article" color="secondary" class="q-ml-sm"
          @click.prevent="onDownloadEmailDataTemplate">
          <q-tooltip>
            下载模板
          </q-tooltip>
        </q-btn>

        <q-btn round dense flat icon="add" class="q-ml-sm" @click.prevent="onSelectExcel" color="grey-7">
          <q-tooltip>
            选择数据
          </q-tooltip>
        </q-btn>
      </div>
    </template>
  </q-field>
</template>

<script lang="ts" setup>
// 模板数据
const modelValue = defineModel({
  type: Array,
  default: () => []
})

// 选择模板数据
import { IExcelColumnMapper, readExcelCore, writeExcel } from 'src/utils/file'
async function onSelectExcel () {
  const { files, sheetName, data } = await readExcelCore({
    sheetIndex: 0,
    selectSheet: true
  })
  if (files) {
    const firstFile = files[0]
    fieldModelValue.value = `${firstFile.name} - ${sheetName}`
  }
  modelValue.value = data
}

// placeholder 显示
import { useCustomQField } from '../helper'
import { notifySuccess } from 'src/utils/notify'
const { isActive, fieldModelValue, fieldText } = useCustomQField('请选择数据 (该项可为空)')

// 删除选择的数据
function onRemoveSelectedFile () {
  fieldModelValue.value = ''
  modelValue.value = []
}

// 下载模板：下载邮件数据模板
function getEmailSendingExcelDataMapper (): IExcelColumnMapper[] {
  return [
    {
      headerName: '收件箱(必填)',
      fieldName: 'inbox',
      required: true
    },
    {
      headerName: '收件人姓名',
      fieldName: 'inboxName'
    },
    {
      headerName: '发件箱',
      fieldName: 'outbox'
    },
    {
      headerName: '发件人姓名',
      fieldName: 'outboxName'
    },
    {
      headerName: '主题',
      fieldName: 'subject'
    },
    {
      headerName: '内容',
      fieldName: 'body'
    },
    {
      headerName: '抄送(多个逗号分隔)',
      fieldName: 'cc'
    },
    {
      headerName: '密送(多个逗号分隔)',
      fieldName: 'bcc'
    },
    {
      headerName: '模板名称',
      fieldName: 'templateName'
    },
    {
      headerName: '模板id',
      fieldName: 'templateId'
    },
    {
      headerName: 'proxyId',
      fieldName: '代理 Id'
    },
    {
      headerName: '自定义xxx',
      fieldName: 'other'
    }
  ]
}
async function onDownloadEmailDataTemplate () {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const data: any[] = [
    {
      inbox: '收件箱(导入时，请删除该行数据)',
      inboxName: '收件人姓名(可选)',
      outbox: '发件箱(可选)',
      outboxName: '发件人姓名(可选)',
      subject: '主题(可选)',
      body: '内容(可选)',
      cc: '抄送(多个逗号分隔,可选)',
      bcc: '密送(多个逗号分隔)(可选)',
      templateName: '模板名称(可选)',
      templateId: '模板id(可选)',
      proxy: '代理Id(可选)',
      other: '可以继续增加列，作为自定义字段(可选)'
    }
  ]
  await writeExcel(data, {
    fileName: '发件数据模板.xlsx',
    sheetName: '发件数据',
    mappers: getEmailSendingExcelDataMapper()
  })

  notifySuccess('模板下载成功，请在下载目录中查看')
}
</script>

<style lang="scss" scoped></style>
