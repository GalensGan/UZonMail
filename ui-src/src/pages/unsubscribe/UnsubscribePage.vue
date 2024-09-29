<template>
  <div class="column justify-start unsubscribe-page_container">
    <div v-html="unsubscribeHtml" :inert="true"></div>
    <CommonBtn class="q-mt-md self-center" :label="btnLabel" @click="onUnsubscribeClicked" />
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

onMounted(async () => {
  // 若有传递 unsubscribeId, 则请求 unsubscribeId 对应的 html
  if (props.htmlContent) {
    unsubscribeHtml.value = props.htmlContent
  }
})

const unsubscribeHtml = ref('<div>empty</div>')
const btnLabel = ref(t('unsuscribePage.unsubscribe'))

async function onUnsubscribeClicked () {
  btnLabel.value = t('unsuscribePage.unsubscribed')
}
</script>

<style lang="scss" scoped>
.unsubscribe-page_container {
  min-width: 400px;
  min-height: 300px;
  width: 100%;
  height: 100%;
  padding: 20px;
  overflow: auto;
}
</style>
