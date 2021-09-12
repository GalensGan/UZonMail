<script>
import { okCancle } from '@/components/iPrompt'

import { upsertTemplate, getTemplate } from '@/api/template'
import _ from 'lodash'

import { toPng } from 'html-to-image'

export default {
  data() {
    return {
      template: {
        html: ''
      }
    }
  },

  async mounted() {},

  methods: {
    hasChange() {
      // 获取内容
      const content = this.getContent()

      // console.log('hasChange:', content, this.lastSavedData)
      return content !== this.lastSavedData
    },

    async generateTemplate() {
      // 更新模板
      const template = _.cloneDeep(this.template)
      template.html = this.getContent()
      // 生成element
      const elem = document.createElement('div')
      elem.innerHTML = template.html
      const parent = document.getElementById('html2image')
      parent.prepend(elem)

      // 生成缩略图
      template.imageUrl = await toPng(elem)

      console.log('generateTemplate:', template.imageUrl)
      return template
    },

    // 新增
    async newTemplate() {
      // 判断数据是否变化，且没有保存
      if (this.hasChange() && this.template._id) {
        const ok = await okCancle('模板已经被修改，是否保存并继续？')
        if (ok) {
          const template = this.generateTemplate()
          // 更新
          await upsertTemplate(template.id, template)
        }
      }

      // 新建模板
      this.setContent('')
      // 清空数据
      this.template = {
        html: ''
      }
    },

    // 保存
    async saveTemplate() {
      // 检查是否是否有id，如果有id，调用保存，否则让用户输入邮件的主题
      if (this.template._id) {
        const ok = await okCancle('是否保存数据?')
        if (!ok) return

        // 开始保存
        const template = this.generateTemplate()
        // 更新
        await upsertTemplate(template.id, template)
      } else {
        // 打开保存框，让用户输入主题
      }
    },

    // 另存为

    async saveAsTemplate() {},

    // 退出
    async exitEditor() {},

    async getTemplateData() {
      // 界面中查看是否有id
      const id = this.$route.query.id
      if (!id) return

      // 获取 id 模板数据
      const res = await getTemplate(id)
      this.template = res.data
      // console.log('mounted:', this.template)

      // 将模板显示到界面上
      this.$nextTick(() => this.setContent(this.template.html))
    }
  }
}
</script>
