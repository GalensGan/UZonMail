<template>
  <div class="column items-center justify-center full-height">
    <div class="column justify-start">
      <div>
        <span>授权类型:</span>
        <span class="text-primary text-subtitle1 text-bold q-ml-sm">{{ activeInfo.licenseType }}</span>
      </div>

      <div>
        <span>激活时间:</span>
        <span class="q-ml-sm">{{ formatDate(activeInfo.activeTime) }}</span>
      </div>

      <div>
        <span>到期时间:</span>
        <span class="q-ml-sm">{{ formatDate(activeInfo.expireTime) }}</span>
      </div>

      <div>
        <span>授权状态:</span>
        <q-chip class="q-ml-sm" outline square dense color="primary" text-color="white" :label="activeInfo.status" />
      </div>
    </div>

    <q-input standout="bg-primary" class="q-mt-md" label-color="white" v-model="license" dense @focus="onFocus"
      @blur="onBlur" :label="licenseLabel">
      <template v-slot:append>
        <q-icon v-if="showActiveIcon" name="motion_photos_auto" @click="onActiveLicense" class="cursor-pointer"
          :color="activeIconColor">
          <AsyncTooltip :tooltip="getActiveIconTooltip" :cache="false"></AsyncTooltip>
        </q-icon>
      </template>
    </q-input>

  </div>
</template>

<script lang="ts" setup>
import AsyncTooltip from 'src/components/asyncTooltip/AsyncTooltip.vue'
import { confirmOperation, notifySuccess } from 'src/utils/dialog'

const license = ref<string>('')

const showActiveIcon = ref(false)
const licenseLabel: Ref<undefined | string> = ref('')
function onFocus () {
  showActiveIcon.value = true
  licenseLabel.value = '请输入授权码'
}
function onBlur () {
  showActiveIcon.value = false
  licenseLabel.value = undefined
}

// 验证激活码是否合法
const isLicenseValid = computed(() => {
  return license.value.length === 24
})
const activeIconColor = computed(() => {
  return isLicenseValid.value ? 'positive' : 'white'
})
function getActiveIconTooltip () {
  return isLicenseValid.value ? '单击激活' : '激活码长度不满足要求'
}

// 激活激活码
async function onActiveLicense () {
  const confirm = await confirmOperation('升级确认', '即将进行升级, 是否继续?')
  if (!confirm) return

  notifySuccess('您已升级成功, 刷新后生效')
}

// #region  激活信息
const activeInfo = ref({
  activeTime: '2021-08-01 12:00:00',
  activeUser: 'admin',
  expireTime: '2022-08-01 12:00:00',
  licenseType: '企业版',
  licenseIcon: 'mdi-crown',
  status: '已激活'
})
import dayjs from 'dayjs'
function formatDate (datetime: string) {
  if (!datetime) return
  return dayjs(datetime).format('YYYY-MM-DD HH:mm:ss')
}
// #endregion
</script>

<style lang="scss" scoped></style>
