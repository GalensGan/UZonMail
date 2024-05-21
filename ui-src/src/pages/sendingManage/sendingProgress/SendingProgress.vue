<template>
  <q-dialog ref='dialogRef' @hide="onDialogHide" :persistent='true'>
    <q-card class='column justify-start q-pa-sm'>
      <div class="text-subtitle1 q-mb-md text-primary">{{ title }}</div>

      <LinearProgress size="40px" :value="progressValue" :width="400" />

      <div class="row justify-end q-mt-md q-gutter-sm">
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

// 发送进度
import { subscribeOne } from 'src/signalR/signalR'
import { UzonMailClientMethods, ISendingGroupProgressArg } from 'src/signalR/types'
import { confirmOperation, notifySuccess } from 'src/utils/notify'
async function onEmailGroupSendingProgressChanged (progress: ISendingGroupProgressArg) {
  console.log(progress)

  if (!progress) return
  if (progress.sendingGroupId !== sendingGroupIdRef.value) return

  // 更新进度
  progressValue.value = progress.current / progress.total

  if (progress.total === progress.current) {
    // 关闭弹窗
    onDialogCancel()
    notifySuccess('发送完成')
  }
}
subscribeOne(UzonMailClientMethods.SendingGroupProgressChanged, onEmailGroupSendingProgressChanged)

// 取消发件
async function OnCancelSending () {
  const confirm = await confirmOperation('取消发件', '确定取消发件吗？')
  if (!confirm) return
  // 开始取消

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

  isSendingPause.value = !isSendingPause.value
}
const router = useRouter()
async function onSendBackGround () {
  // 关闭当前操作
  onDialogOK()

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
