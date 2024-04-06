<template>
  <el-dropdown>
    <UserAvatar />
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item>个人信息</el-dropdown-item>
        <el-dropdown-item @click="onLogout">退出</el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script lang="ts" setup>
import UserAvatar from 'src/components/userAvatar/UserAvatar.vue'

import { useUserInfoStore } from 'src/stores/user'
const userInfoStore = useUserInfoStore()
function onLogout () {
  userInfoStore.logout()
}
</script>

<style lang="scss" scoped></style>
