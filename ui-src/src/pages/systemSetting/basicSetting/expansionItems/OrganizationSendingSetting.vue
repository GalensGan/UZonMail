<template>
  <SendSetting icon="local_shipping" label="组织发件设置" caption="当子账户为跟随模式时，所有账户应用该设置"
    :get-setting-api="onGetCurrentDepartmentSetting" :update-setting-api="onUpdateDepartmentSetting" />
</template>

<script lang="ts" setup>
import SendSetting from './SendSetting.vue'
import { useUserInfoStore } from 'src/stores/user'
import { getCurrentDepartmentSetting, updateDepartmentSetting } from 'src/api/pro/departmentSetting'
import { IOrganizationSetting } from 'src/api/organizationSetting'

const userInfoStore = useUserInfoStore()
async function onGetCurrentDepartmentSetting () {
  return await getCurrentDepartmentSetting(userInfoStore.departmentId)
}

async function onUpdateDepartmentSetting (userSetting: IOrganizationSetting) {
  return await updateDepartmentSetting(userInfoStore.departmentId, userSetting)
}
</script>

<style lang="scss" scoped></style>
