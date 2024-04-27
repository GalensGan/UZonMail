<template>
  <q-linear-progress size="20px" :value="progressValue" class="q-mr-md border-radius-4" color="secondary"
    style="max-width:200px">
    <div class="absolute-full flex flex-center">
      <q-badge color="white" text-color="accent" :label="progressLabel" />
    </div>
  </q-linear-progress>
</template>

<script lang="ts" setup>
// 进度条
const progressValue = ref(0.5)
const progressLabel = ref('0.00%')

import { useNotifyRegister } from './notifyFromServer'
useNotifyRegister()

import { useSendEmailHub } from 'src/compositions/signalR'
onMounted(() => {
  const sendMailHub = useSendEmailHub()
  // 注册事件
  sendMailHub.on('Notify', (message: string) => {
    console.log('Notify: 1', message)
  })
  sendMailHub.on('notify', (message: string) => {
    console.log('notify: 2', message)
  })
})
onUnmounted(() => {
  // 移除注册
})
</script>
<style lang="scss" scoped></style>
