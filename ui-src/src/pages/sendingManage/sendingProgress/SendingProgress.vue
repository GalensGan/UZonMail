<template>
  <q-dialog ref='dialogRef' :persistent='true'>
    <q-card class='column justify-start q-pa-sm'>
      <div class="text-subtitle1 q-mb-md text-primary">{{ title }}</div>

      <LinearProgress size="40px" :value="progressValue" :width="400" />

      <div class="row justify-end q-mt-md q-gutter-sm">
        <CancelBtn @click="onDialogOK" tooltip="取消发件" />
        <CommonBtn label="暂停" tooltip="暂停发件" />
        <OkBtn @click="onDialogCancel" label="后台" tooltip="在后台发件" />
      </div>
    </q-card>
  </q-dialog>
</template>

<script lang='ts' setup>
/**
 * warning: 该组件是一个弹窗的示例，不可直接使用
 * 参考：http://www.quasarchs.com/quasar-plugins/dialog#composition-api-variant
 */

import { useDialogPluginComponent } from 'quasar'
defineEmits([
  // 必需；需要指定一些事件
  // （组件将通过useDialogPluginComponent()发出）
  ...useDialogPluginComponent.emits
])
const { dialogRef, onDialogOK, onDialogCancel } = useDialogPluginComponent()

import LinearProgress from 'src/components/Progress/LinearProgress.vue'
import CommonBtn from 'src/components/componentWrapper/buttons/CommonBtn.vue'
import OkBtn from 'src/components/componentWrapper/buttons/OkBtn.vue'
import CancelBtn from 'src/components/componentWrapper/buttons/CancelBtn.vue'

const props = defineProps({
  // 发件组 id
  // 若有，说明是查看
  sendingGroupId: {
    type: Number
  },

  // 发件接口
  sendingApi: {
    type: Function
  },

  title: {
    type: String,
    default: () => '发送进度'
  }
})

const sendingGroupIdRef = ref(props.sendingGroupId)
onMounted(async () => {
  // 当有 api 时，说明是新增，先调用 api
  if (typeof props.sendingApi === 'function') {
    const { data: groupId } = await props.sendingApi()
    sendingGroupIdRef.value = groupId
  }

  if (!sendingGroupIdRef.value) return

  // 有 id 时，获取 id 信息
  console.log(sendingGroupIdRef.value)
})

const progressValue = ref(0)

import { subscribeOne } from 'src/compositions/signalR'
function onEmailGroupSendingProgressChanged (progress: {
  startDate: string,
  sendingGroupId: number,
  total: number,
  current: number,
  message?: string
}) {
  if (!progress) return
  if (progress.sendingGroupId !== sendingGroupIdRef.value) return

  console.log(progress)
  // 更新进度
  progressValue.value = progress.current / progress.total
}
subscribeOne('emailGroupSendingProgressChanged', onEmailGroupSendingProgressChanged)
</script>

<style lang='scss' scoped></style>
