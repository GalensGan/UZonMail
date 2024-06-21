<template>
  <q-dialog ref='dialogRef' @hide="onDialogHide" :persistent='true'>
    <q-card class='column justify-start q-pa-sm'>
      <div class="text-subtitle1 q-mb-md text-primary">{{ title }}</div>

      <LinearProgress size="40px" :value="progressValue" :width="400" />

      <div v-if="existSendingGroupId" class="row justify-end q-mt-md q-gutter-sm">
        <CancelBtn @click="OnCancelSending" tooltip="取消发件" />
        <CommonBtn @click="onToggleTaskSending" :label="toggleLabel" :tooltip="toggleTooltip" />
        <OkBtn @click="onSendBackGround" label="后台" tooltip="在后台发件" />
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
const { dialogRef, onDialogOK, onDialogCancel, onDialogHide } = useDialogPluginComponent()

import LinearProgress from 'src/components/Progress/LinearProgress.vue'
import CommonBtn from 'src/components/componentWrapper/buttons/CommonBtn.vue'
import OkBtn from 'src/components/componentWrapper/buttons/OkBtn.vue'
import CancelBtn from 'src/components/componentWrapper/buttons/CancelBtn.vue'

const props = defineProps({
  // 发件组 id
  // 若有，说明是查看
  sendingGroupId: {
    type: Number,
    required: false
  },

  // 发件接口
  sendingApi: {
    type: Function,
    required: false
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
    try {
      const { data: groupDoc } = await props.sendingApi()
      sendingGroupIdRef.value = groupDoc.id
    } catch (error) {
      console.log(error)
      // 关闭进度
      onDialogCancel()
    }
  }

  if (!sendingGroupIdRef.value) return

  // 有 id 时，获取 id 信息
  console.log(sendingGroupIdRef.value)
})
const existSendingGroupId = computed(() => {
  return sendingGroupIdRef.value
})

const progressValue = ref(0)

// 发送进度
import { subscribeOne } from 'src/signalR/signalR'
import { UzonMailClientMethods, ISendingGroupProgressArg, SendingGroupProgressType } from 'src/signalR/types'
import { confirmOperation, notifySuccess } from 'src/utils/dialog'
async function onEmailGroupSendingProgressChanged (progress: ISendingGroupProgressArg) {
  console.log('onEmailGroupSendingProgressChanged', progress)
  if (!progress) return
  if (progress.sendingGroupId !== sendingGroupIdRef.value) return
  if (progress.progressType === SendingGroupProgressType.end) {
    onDialogCancel()

    notifySuccess(`邮件组 ${progress.sendingGroupId} 发送完成`)
    return
  }

  // 更新进度
  progressValue.value = progress.current / progress.total
}
subscribeOne(UzonMailClientMethods.sendingGroupProgressChanged, onEmailGroupSendingProgressChanged)

// 取消发件
import { cancelSending, pauseSending, restartSending } from 'src/api/emailSending'
async function OnCancelSending () {
  const confirm = await confirmOperation('取消发件', '确定取消发件吗？')
  if (!confirm) return
  // 开始取消
  await cancelSending(sendingGroupIdRef.value as number)
  notifySuccess('取消成功')
  // 关闭窗体
  onDialogCancel()
}

const isSendingPause = ref(false)
const toggleLabel = computed(() => {
  return isSendingPause.value ? '继续' : '暂停'
})
const toggleTooltip = computed(() => {
  return isSendingPause.value ? '继续发件' : '暂停发件'
})
async function onToggleTaskSending () {
  // 向服务器发送暂停/继续请求
  if (isSendingPause.value) {
    await restartSending(sendingGroupIdRef.value as number)
  } else {
    await pauseSending(sendingGroupIdRef.value as number)
  }

  isSendingPause.value = !isSendingPause.value
}
const router = useRouter()
async function onSendBackGround () {
  // 关闭当前操作
  onDialogOK()

  console.log('发送到后台', sendingGroupIdRef.value)
  // 切换到明细中
  router.push({
    name: 'SendDetailTable',
    query: {
      sendingGroupId: sendingGroupIdRef.value,
      tagName: sendingGroupIdRef.value
    }
  })
}
</script>

<style lang='scss' scoped></style>
