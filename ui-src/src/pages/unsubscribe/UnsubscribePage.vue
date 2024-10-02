<template>
  <div class="column unsubscribe-page_container">
    <div v-html="unsubscribeHtml" :inert="true"></div>
    <CommonBtn class="q-mt-xl self-center" :label="btnLabel" @click="onUnsubscribeClicked" />
  </div>
</template>

<script lang="ts" setup>
import CommonBtn from 'src/components/componentWrapper/buttons/CommonBtn.vue'
import { useI18n } from 'vue-i18n'
const { t } = useI18n()

const props = defineProps({
  htmlContent: {
    type: String,
    required: false,
    default: ''
  }
})

const token = ref('')
const unsubscribeId = ref(0)

import { useRoute } from 'vue-router'
import { useQuasar } from 'quasar'
import { getUnsubscribePage } from 'src/api/pro/unsubscribePage'

const route = useRoute()
const $q = useQuasar()
onMounted(async () => {
  // 若有传递 unsubscribeId, 则请求 unsubscribeId 对应的 html
  if (props.htmlContent) {
    unsubscribeHtml.value = props.htmlContent
    return
  }

  // 获取当前语言，根据语言获取对应的退订页面
  unsubscribeId.value = Number(route.query.unsubscribeId) || 0
  const { data } = await getUnsubscribePage(unsubscribeId.value, $q.lang.getLocale())
  unsubscribeHtml.value = data.htmlContent

  // 获取 token
  token.value = route.query.token as string
})

const unsubscribeHtml = ref('<div>empty</div>')
const btnLabel = ref(t('unsuscribePage.unsubscribe'))

import { unsubscribe } from 'src/api/pro/unsubscribe'

async function onUnsubscribeClicked () {
  btnLabel.value = t('unsuscribePage.unsubscribed')
  if (!token.value) {
    return
  }

  // 发送退订请求
  await unsubscribe(token.value)
}
</script>

<style lang="scss" scoped>
.unsubscribe-page_container {
  min-width: 400px;
  width: 100%;
  height: 100%;
  padding: 20px;
  overflow: auto;
}
</style>
