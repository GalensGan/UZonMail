<template>
  <q-expansion-item popup icon="account_tree" label="组织基础设置" caption="设置组织配置应用模式"
    header-class="text-primary card-like-borderless" @before-show="onBeforeShow" group="settings1">
    <div class="q-pa-md">
      <div class="column justify-start q-mb-sm ">
        <div class="row justify-start items-center">
          <span>子账户策略</span>
          <div class="q-gutter-sm q-ml-sm">
            <q-radio dense v-model="departmentSetting.subUserStrategy" :val="0" label="独立">
              <AsyncTooltip tooltip="子账户可独立管理自己的设置" />
            </q-radio>
            <q-radio dense v-model="departmentSetting.subUserStrategy" :val="1" label="跟随">
              <AsyncTooltip tooltip="子账户统一采用组织设置" />
            </q-radio>
          </div>
        </div>
      </div>
    </div>
  </q-expansion-item>
</template>

<script lang="ts" setup>
import AsyncTooltip from 'src/components/asyncTooltip/AsyncTooltip.vue'
import { useUserInfoStore } from 'src/stores/user'
import { IDepartmentSetting, getDepartmentSetting, updateDepartmentSetting } from 'src/api/department'
import { setTimeoutAsync } from 'src/utils/tsUtils'
import logger from 'loglevel'

const departmentSetting: Ref<IDepartmentSetting> = ref({
  departmentId: 0,
  subUserStrategy: 0
})

// 获取设置
const userInfoStore = useUserInfoStore()
const isInitializing = ref(false)
async function onBeforeShow () {
  // 获取设置
  logger.debug('[setting] request department setting')

  const { data } = await getDepartmentSetting(userInfoStore.departmentId)
  isInitializing.value = true
  departmentSetting.value = data
  await setTimeoutAsync(10)
  isInitializing.value = false
}
// 自动保存更新
watch(departmentSetting, async (newVal) => {
  if (isInitializing.value) return

  await updateDepartmentSetting(newVal)
}, { deep: true })
</script>

<style lang="scss" scoped></style>
