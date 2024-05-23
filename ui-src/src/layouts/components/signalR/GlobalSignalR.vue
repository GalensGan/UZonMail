<template>
  <div v-show="showProgress" class="q-mr-md cursor-pointer">
    <LinearProgress :progress-value="progressValue" size="18px" :width="160">
    </LinearProgress>
    <q-menu v-if="sendingGroups.length > 0" v-model="showMore" :offset="[0, 8]">
      <q-list dense style="min-width: 160px" class="q-px-sm q-py-xs">
        <q-item class="border-radius-4" v-for="item in sendingGroups" :key="item.id" clickable v-close-popup
          @click="showSendingGroupDetail(item)">
          <q-item-section class="text-subtitle max-width-200 ellipsis">
            {{ item.subjects }}
          </q-item-section>
          <q-item-section side class="text-primary">{{ getSendingGroupProgressLabel(item) }}</q-item-section>
        </q-item>
      </q-list>
    </q-menu>
  </div>
</template>

<script lang="ts" setup>
// 进度条
import LinearProgress from 'src/components/Progress/LinearProgress.vue'

// 打开详细进度弹窗
// eslint-disable-next-line @typescript-eslint/no-explicit-any
import { IRunningSendingGroup, getRunningSendingGroups } from 'src/api/sendingGroup'
const sendingGroups: Ref<IRunningSendingGroup[]> = ref([])
const showMore = ref(false)
watch(sendingGroups, () => {
  if (showMore.value) {
    showMore.value = sendingGroups.value.length > 0
  }
})
async function onOpenSendingDetail () {
  const { data } = await getRunningSendingGroups()
  // 获取正在发送的邮箱组
  sendingGroups.value = data || []
}
const progressValue = computed(() => {
  // 计算总进度
  return sendingGroups.value.reduce((prev, current) => prev + current.progress, 0) / sendingGroups.value.length
})
const showProgress = computed(() => {
  return progressValue.value - 1 > -0.0001 || progressValue.value < 0.0001
})
onMounted(async () => {
  // 获取当前用户的发送进度
  await onOpenSendingDetail()
})

function getSendingGroupProgressLabel (sendingGroupInfo: IRunningSendingGroup) {
  return `${(sendingGroupInfo.progress * 100).toFixed(1)}%`
}
const router = useRouter()
function showSendingGroupDetail (sendingGroupInfo: IRunningSendingGroup) {
  router.push({
    name: 'SendDetailTable',
    query: {
      sendingGroupId: sendingGroupInfo.id,
      tagName: sendingGroupInfo.id
    }
  })
}

// 注册消息
import { useNotifyRegister } from './notifyFromServer'
useNotifyRegister()

// 注册总进度回调
import { subscribeOne } from 'src/signalR/signalR'
import { IGroupEndSendingArg, IGroupStartSendingArg, ISendingGroupProgressArg, IUserSendingProgressArg, UzonMailClientMethods } from 'src/signalR/types'
function onUserSendingProgressChanged (arg: IUserSendingProgressArg) {
  // 更新进度条
  progressValue.value = arg.current / arg.total
}
subscribeOne(UzonMailClientMethods.userSendingProgressChanged, onUserSendingProgressChanged)

// 注册单个发件组进度回调
function onSendingGroupProgressChanged (arg: ISendingGroupProgressArg) {
  // 更新单个进度
  const index = sendingGroups.value.findIndex(item => item.id === arg.sendingGroupId)
  if (index < 0) return

  // 更新值
  sendingGroups.value[index].totalCount = arg.total
  sendingGroups.value[index].sentCount = arg.current
  sendingGroups.value[index].progress = arg.current * 1.0 / arg.total
}
subscribeOne(UzonMailClientMethods.sendingGroupProgressChanged, onSendingGroupProgressChanged)

// 注册开始发件
function onGroupStartSending (arg: IGroupStartSendingArg) {
  // 添加到发送组列表
  sendingGroups.value.push({
    id: arg.sendingGroupId,
    subjects: arg.subjects,
    progress: 0,
    totalCount: arg.total,
    sentCount: arg.current,
    successCount: 0
  })
}
subscribeOne(UzonMailClientMethods.groupStartSending, onGroupStartSending)
// 注册结束发件
function onGroupEndSending (arg: IGroupEndSendingArg) {
  const index = sendingGroups.value.findIndex(item => item.id === arg.sendingGroupId)
  if (index < 0) return

  // 移除
  sendingGroups.value.splice(index, 1)
}
subscribeOne(UzonMailClientMethods.groupEndSending, onGroupEndSending)

</script>
<style lang="scss" scoped></style>
