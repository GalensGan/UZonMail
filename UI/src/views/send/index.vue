<template>
  <div class="q-pa-lg send-container q-gutter-md">
    <div class="receive-box row">
      <strong style="height: auto; align-self: center"> 主题：</strong>
      <input type="text" class="send-input col-grow" v-model="subject" />
    </div>
    <div class="receive-box row justify-between">
      <div class="row col-grow">
        <strong style="height: auto; align-self: center"> 收件人：</strong>
        <q-chip
          v-for="rec in receivers"
          :key="rec.type + rec._id"
          removable
          @remove="removeReceiver(rec)"
          :color="rec.type === 'group' ? 'orange' : 'teal'"
          size="sm"
          text-color="white"
          :label="rec.label"
        />
        <input type="text" class="send-input col-grow" />
      </div>
      <q-btn
        size="sm"
        dense
        class="self-center q-mb-sm"
        color="orange"
        outline
        @click="openSelectReceiversDialog"
        label="选择收件人"
      />
    </div>

    <div class="row justify-between">
      <div class="row content-center">
        <strong style="height: auto; align-self: center"> 模板：</strong>
        <el-select
          v-model="selectedTemplate"
          placeholder="请选择"
          size="small"
          value-key="_id"
        >
          <el-tooltip
            v-for="item in options"
            :key="item._id"
            class="item"
            effect="light"
            placement="right"
          >
            <div slot="content">
              <q-img
                class="rounded-borders"
                :src="item.imageUrl"
                width="300px"
              />
            </div>
            <el-option :label="item.name" :value="item"> </el-option>
          </el-tooltip>
        </el-select>
      </div>
      <div
        class="receive-box row justify-between content-center q-ml-md"
        style="flex: 1"
      >
        <div>
          <strong style="height: auto; align-self: center"> 数据：</strong
          >{{ selectedFileName }}
        </div>
        <input
          type="file"
          id="fileInput"
          style="display: none"
          accept="application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          @change="fileSelected"
        />
        <q-btn
          label="选择文件"
          dense
          size="sm"
          outline
          color="orange"
          class="q-mb-sm"
          @click="selectExcelFile"
        ></q-btn>
      </div>
    </div>

    <div v-html="selectedTemplate.html"></div>

    <div class="row justify-end preview-row">
      <q-btn
        label="发送"
        color="primary"
        size="sm"
        class="q-mr-sm"
        @click="startSending"
      />
      <q-btn label="预览" color="orange" size="sm" @click="previewEmailBody" />
    </div>

    <q-dialog v-model="isShowSendingDialog" persistent>
      <SendingProgress @close="isShowSendingDialog = false" />
    </q-dialog>

    <q-dialog v-model="isShowPreviewDialog" persistent>
      <q-card style="max-width: none">
        <q-layout
          view="lHh lpr lFf"
          container
          style="height: 400px; width: 600px"
          class="shadow-2 rounded-borders"
        >
          <q-header elevated class="bg-teal">
            <div class="q-pa-sm text-subtitle1">
              发送给：{{ previewData.receiverName }}/{{
                previewData.receiverEmail
              }}
            </div>
          </q-header>

          <q-footer elevated class="bg-teal">
            <div class="row justify-between q-pa-sm">
              <div>
                当前：{{ previewData.index + 1 }} / 合计：{{
                  previewData.total
                }}
              </div>
              <div class="row justify-end q-gutter-sm">
                <q-btn
                  label="上一条"
                  color="orange"
                  size="sm"
                  @click="previousItem"
                />
                <q-btn
                  label="下一条"
                  color="orange"
                  size="sm"
                  @click="nextItem"
                />
                <q-btn label="退出" color="negative" size="sm" v-close-popup />
              </div>
            </div>
          </q-footer>

          <q-page-container>
            <div v-html="previewData.html"></div>
          </q-page-container>
        </q-layout>
      </q-card>
    </q-dialog>

    <q-dialog v-model="isShowSelectEmails">
      <SelectEmail groupType="receive" v-model="receivers" />
    </q-dialog>
  </div>
</template>

<script>
import { getTemplates } from '@/api/template'
import {
  newPreview,
  getPreviewData,
  getCurrentStatus,
  newSendTask,
  startSending
} from '@/api/send'

import XLSX from 'js-xlsx'
import { notifyError, okCancle } from '@/components/iPrompt'
import SelectEmail from './components/selectEmail'
import SendingProgress from './components/sendingProgress'

export default {
  components: { SelectEmail, SendingProgress },
  data() {
    return {
      subject: '',
      receivers: [],
      selectedFileName: '',
      options: [],
      selectedTemplate: {},

      isShowSendingDialog: false,

      isShowPreviewDialog: false,
      previewData: {},

      sendingData: {
        progress: 0,
        label: '0.00%'
      },

      isShowSelectEmails: false
    }
  },
  async mounted() {
    // 获取当前状态，可能处理发送中
    const statusRes = await getCurrentStatus()
    if (statusRes.data > 1) {
      // 打开发送框
      this.isShowSendingDialog = true
    }

    // 获取所有模板
    const res = await getTemplates()
    this.options = res.data

    // 设置默认选项
    if (res.data && res.data.length > 0) this.selectedTemplate = res.data[0]
  },
  methods: {
    // 选择文件
    selectExcelFile() {
      const elem = document.getElementById('fileInput')
      elem.click()
      elem.value = ''
    },

    async fileSelected(e) {
      console.log('fileSelected:', e)
      // 判断是否选择了文件
      if (e.target.files.length === 0) {
        return
      }

      // 获取选择的文件
      const file = e.target.files[0]

      this.excelData = await this.readExcelData(file)
      if (!this.excelData) return
      this.selectedFileName = file.name
    },

    async readExcelData(file) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = e => {
          const data = new Uint8Array(e.target.result)
          const workbook = XLSX.read(data, { type: 'array' })
          /* DO SOMETHING WITH workbook HERE */
          // 变成json
          const jsonObj = XLSX.utils.sheet_to_json(
            workbook.Sheets[workbook.SheetNames[0]]
          )
          resolve(jsonObj)
        }
        reader.onerror = () => {
          reject(false)
        }

        reader.readAsArrayBuffer(file)
      })
    },

    async checkData() {
      if (!this.subject) {
        notifyError('请输入主题')
        return false
      }

      if (!this.receivers || this.receivers.length < 1) {
        notifyError('请选择收件人')
        return false
      }

      if (!this.excelData) {
        notifyError('请选择模板数据')
        return false
      }

      return true
    },

    // 预览邮箱
    async previewEmailBody() {
      // 判断数据输入
      const isNewTask = await this.checkData()
      if (!isNewTask) return

      // 新建预览
      await newPreview(
        this.subject,
        this.receivers,
        this.excelData,
        this.selectedTemplate._id
      )

      // 打开预览窗体
      this.isShowPreviewDialog = true

      // 获取第一个预览
      const res = await getPreviewData('first')
      this.previewData = res.data
    },

    async previousItem() {
      const res = await getPreviewData('previous')
      this.previewData = res.data
    },

    async nextItem() {
      const res = await getPreviewData('next')
      this.previewData = res.data
    },

    async startSending() {
      // 判断数据输入
      const isNewTask = await this.checkData()
      if (!isNewTask) return

      // 新建发件任务
      const res = await newSendTask(
        this.subject,
        this.receivers,
        this.excelData,
        this.selectedTemplate._id
      )

      const { data } = res
      if (!data.ok) {
        notifyError(data.message)
        return
      }

      const ok = await okCancle(
        '发件确认',
        `选择收件人：${data.selectedReceiverCount}位，录入数据：${data.dataReceiverCount}条，实际发件：${data.acctualReceiverCount}位。是否继续？`
      )
      if (!ok) return

      // 开始发送
      await startSending(data.historyId)

      this.isShowSendingDialog = true
    },

    openSelectReceiversDialog() {
      this.isShowSelectEmails = true
    },

    removeReceiver(receiver) {
      const index = this.receivers.findIndex(
        re => re.type === receiver.type && re._id === receiver._id
      )
      if (index > -1) this.receivers.splice(index, 1)
    }
  }
}
</script>

<style lang='scss'>
.send-container {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  overflow-y: auto;

  .receive-box {
    border-bottom: 1px solid gray;
  }

  .send-input {
    border: none;
    outline: none;
  }

  .preview-row {
    position: fixed;
    right: 20px;
    bottom: 20px;
  }
}
</style>