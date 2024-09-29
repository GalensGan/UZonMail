<template>
  <q-expansion-item popup icon="unsubscribe" label="退订设置" caption="进行退订相关的设置"
    header-class="text-primary card-like-borderless" @before-show="onBeforeShow" group="settings1">
    <div class="q-pa-md">
      <div class="column justify-start q-mb-sm ">
        <div class="row justify-start items-center">
          <span>启用退订</span>
          <q-checkbox class="q-ml-sm" color="secondary" keep-color v-model="unsubscribeSetting.enable"
            left-label></q-checkbox>
        </div>

        <div v-if="unsubscribeSetting.enable" class="row justify-start items-center">
          <div>退订回调</div>

          <div class="q-gutter-sm q-ml-sm">
            <q-radio dense v-model="unsubscribeSetting.type" :val="0" label="系统">
              <AsyncTooltip tooltip="使用系统本身提供的退订功能进行管理" />
            </q-radio>
            <q-radio dense v-model="unsubscribeSetting.type" :val="1" label="外链">
              <AsyncTooltip tooltip="使用外部链接接受退订回调" />
            </q-radio>
          </div>

          <q-input v-if="unsubscribeSetting.type" class="q-ml-md" outlined standout
            v-model="unsubscribeSetting.externalUrl" dense label="退订链接">
            <template v-slot:prepend>
              <q-icon name="http" color="secondary" />
            </template>
          </q-input>
        </div>

        <div v-if="enableUnsubscribePageSetting" class="row justify-start items-start">
          <div class="q-mt-sm q-ml-sm">退订页面</div>
          <UnsubscribePage class="col" />
        </div>
      </div>
    </div>
  </q-expansion-item>
</template>

<script lang="ts" setup>
import AsyncTooltip from 'src/components/asyncTooltip/AsyncTooltip.vue'
import UnsubscribePage from './components/UnsubscribePage.vue'

import { useUserInfoStore } from 'src/stores/user'
import { setTimeoutAsync } from 'src/utils/tsUtils'
import logger from 'loglevel'

const unsubscribeSetting = ref({
  enable: false,
  type: 0,
  externalUrl: ''
})

// 获取设置
const userInfoStore = useUserInfoStore()
const isInitializing = ref(false)
async function onBeforeShow () {
  // 获取设置
  logger.debug('[setting] request unsubescribe setting', userInfoStore)

  isInitializing.value = true
  await setTimeoutAsync(10)
  isInitializing.value = false
}

watch(unsubscribeSetting, async (newVal) => {
  if (isInitializing.value) return
  logger.debug('[setting] update unsubscribe setting', newVal)
}, { deep: true })

const enableUnsubscribePageSetting = computed(() => {
  return unsubscribeSetting.value.enable && unsubscribeSetting.value.type === 0
})
</script>

<style lang="scss" scoped></style>
