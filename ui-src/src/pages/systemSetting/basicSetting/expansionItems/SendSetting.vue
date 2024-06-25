<template>
  <q-expansion-item popup icon="flight_takeoff" label="发件设置" caption="设置发件间隔、最大发件量等"
    header-class="text-primary card-like-borderless" @after-show="onAfterShow">
    <q-card>
      <q-card-section>
        <div class="row justify-start items-center q-mb-sm ">
          <q-input outlined class="col" standout dense v-model="outboxSettingRef.maxSendCountPerEmailDay"
            :debounce="500" type="number" label="单个发件箱每日最大发件量" placeholder="为 0 时表示不限制">
          </q-input>

          <q-input outlined class="q-ml-sm col" standout dense v-model="outboxSettingRef.maxSendingBatchSize"
            :debounce="500" type="number" label="合并发件最大数量" placeholder="为 0 时表示不合并">
          </q-input>
        </div>

        <div class="row justify-start items-center q-mb-sm ">
          <q-input outlined class="col" standout dense v-model="outboxSettingRef.minOutboxCooldownSecond" type="number"
            :debounce="500" label="单个发件箱最小发件间隔 (单位: 秒)" placeholder="为 0 时表示不限制">
          </q-input>

          <q-input outlined class="q-ml-sm col" standout dense v-model="outboxSettingRef.maxOutboxCooldownSecond"
            type="number" :debounce="500" label="单个发件箱最大发件间隔 (单位: 秒)" placeholder="为 0 时表示不限制">
          </q-input>
        </div>

        <div class="row justify-start items-center q-mb-sm ">
          <q-input outlined class="col" standout dense v-model="outboxSettingRef.minInboxCooldownHours" type="number"
            :debounce="500" label="最短收件间隔 (单位: h)" placeholder="为 0 时表示不限制">
          </q-input>

          <q-input outlined class="q-ml-sm col" standout dense v-model="outboxSettingRef.replyToEmails" :debounce="500"
            label="回信收件人" placeholder="收件箱回信后的收信邮箱,若有多个使用逗号分隔">
          </q-input>
        </div>
      </q-card-section>
    </q-card>
  </q-expansion-item>
</template>

<script lang="ts" setup>
import { IUserSetting, getCurrentUserSetting, updateUserSetting } from 'src/api/userSetting'
import { useUserInfoStore } from 'src/stores/user'
const userInfoStore = useUserInfoStore()
const outboxSettingRef: Ref<IUserSetting> = ref({
  userId: userInfoStore.userId,
  maxSendCountPerEmailDay: 0,
  minOutboxCooldownSecond: 5,
  maxOutboxCooldownSecond: 10,
  maxSendingBatchSize: 20,
  minInboxCooldownHours: 0,
  replyToEmails: ''
})
// 获取设置
let updateSettingSignal = true
async function onAfterShow () {
  // 获取设置
  const { data: setting } = await getCurrentUserSetting()
  if (setting) {
    updateSettingSignal = false
    outboxSettingRef.value = setting
  }
}

watch(outboxSettingRef, async () => {
  // 保存设置
  if (!updateSettingSignal) {
    updateSettingSignal = true
    return
  }

  await updateUserSetting(outboxSettingRef.value)
}, { deep: true })
</script>

<style lang="scss" scoped></style>
