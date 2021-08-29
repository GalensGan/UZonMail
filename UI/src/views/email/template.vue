<template>
  <div class="column q-pa-md">
    <q-btn dense color="primary" class="q-mb-sm self-start" @click="selectFile"
      >导入模板</q-btn
    >
    <input
      type="file"
      id="fileInput"
      style="display: none"
      accept="text/html"
      @change="fileSelected"
    />

    <q-card
      v-for="temp in data"
      :key="temp._id"
      flat
      bordered
      class="column q-pa-xs"
      style="width: 400px"
    >
      <div class="text-overline">{{ temp.name }}</div>
      <q-img class="rounded-borders" :src="temp.imageUrl" />
      <div class="row justify-between q-mt-sm">
        <div>创建时间:{{ temp.createDate | formatDate }}</div>
        <q-btn          
          color="negative"
          class="self-center"
          size="sm"
          @click="deleteTemplate(temp._id)"          
          >删除</q-btn
        >
      </div>
    </q-card>

    <q-dialog v-model="isShowTemplateDialog" persistent>
      <q-card class="column" style="max-width: none">
        <div>{{ selectedFileName }}</div>
        <div id="capture" v-html="templateHtml"></div>
        <div class="row justify-end q-ma-sm q-gutter-sm">
          <q-btn color="negative" v-close-popup>取消</q-btn>
          <q-btn color="teal" @click="confirmTemplate">确认</q-btn>
        </div>
      </q-card>
    </q-dialog>
  </div>
</template>

<script>
import html2canvas from 'html2canvas'
import { newTemplate, getTemplates, deleteTemplate } from '@/api/template'
import moment from 'moment'
import { notifySuccess, okCancle } from '@/components/iPrompt'

export default {
  data() {
    return {
      templateHtml: '',
      isShowTemplateDialog: false,
      selectedFileName: '',
      data: []
    }
  },
  filters: {
    formatDate(date) {
      if (!date) return ''

      return moment(date).format('YYYY-MM-DD')
    }
  },

  async mounted() {
    const res = await getTemplates()
    this.data = res.data
  },

  methods: {
    // 选择文件
    selectFile() {
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
      this.selectedFileName = file.name
      this.templateHtml = await this.readExcelData(file)
    },

    async readExcelData(file) {
      return new Promise((resolve, reject) => {
        const reader = new FileReader()
        reader.onload = e => {
          // 获取html
          const result = e.target.result
          this.isShowTemplateDialog = true
          resolve(result)
        }
        reader.onerror = () => {
          reject(false)
        }

        reader.readAsText(file)
      })
    },

    // 确认邮件
    async confirmTemplate() {
      // 生成模板预览图
      const canvas = await html2canvas(document.getElementById('capture'), {
        scale: 1, //缩放比例,默认为1
        allowTaint: false, //是否允许跨域图像污染画布
        useCORS: true, //是否尝试使用CORS从服务器加载图像
        //width: '500', //画布的宽度
        //height: '500', //画布的高度
        backgroundColor: '#000000' //画布的背景色，默认为透明
      })

      //将canvas转为base64格式
      const imageUrl = canvas.toDataURL('image/png')

      // 发送模板
      const res = await newTemplate(
        this.selectedFileName,
        imageUrl,
        this.templateHtml
      )

      // 添加到集合
      this.data.push(res.data)

      notifySuccess('添加成功')

      this.isShowTemplateDialog = false
    },

    // 删除邮件
    async deleteTemplate(id) {
      const ok = await okCancle('是否删除模板？')
      if (!ok) return

      const res = await deleteTemplate(id)

      // 删除成功后，清除显示
      const index = this.data.findIndex(d => d._id === id)
      if (index > -1) this.data.splice(index, 1)

      notifySuccess('删除成功')
    }
  }
}
</script>

<style lang='scss'>
</style>