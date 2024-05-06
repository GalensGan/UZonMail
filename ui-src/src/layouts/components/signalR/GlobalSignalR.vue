<template>
  <div class="q-mr-md cursor-pointer" @click="onOpenSendingDetail">
    <LinearProgress :progress-value="progressValue" size="18px" :width="160">
    </LinearProgress>
    <q-menu v-if="sendingGroups.length > 0" v-model="showMore" :offset="[0, 8]">
      <q-list dense style="min-width: 160px" separator class="q-pa-sm">
        <q-item class="border-radius-4" v-for="item in sendingGroups" :key="item.id" clickable v-close-popup>
          <q-item-section class="text-subtitle">{{ item.subject }}</q-item-section>
          <q-item-section side class="text-secondary">{{ item.sendingStatusLabel }}</q-item-section>
        </q-item>
      </q-list>
    </q-menu>
  </div>
</template>

<script lang="ts" setup>
// 进度条
import LinearProgress from 'src/components/Progress/LinearProgress.vue'
const progressValue = ref(0.5)

import { useNotifyRegister } from './notifyFromServer'
useNotifyRegister()

// 注册事件
// function onSendingGroupProgressChanged () {
// }

// 打开详细进度弹窗
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const sendingGroups: Ref<Record<string, any>[]> = ref([])
const showMore = ref(false)
watch(sendingGroups, () => {
  if (showMore.value) {
    showMore.value = sendingGroups.value.length > 0
  }
})
async function onOpenSendingDetail () {
  // 获取正在发送的邮箱组
  sendingGroups.value = [{
    id: 1,
    subject: '主题',
    sendingStatusLabel: '50%'
  }, {
    id: 2,
    subject: '主题',
    sendingStatusLabel: '50%'
  }]
}
</script>
<style lang="scss" scoped></style>
