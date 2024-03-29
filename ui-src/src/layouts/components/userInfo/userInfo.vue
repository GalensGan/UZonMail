<template>
  <el-dropdown>
    <q-avatar>
      <img src="https://cdn.quasar.dev/img/avatar2.jpg">
    </q-avatar>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item>个人信息</el-dropdown-item>
        <el-dropdown-item @click="onLogout">退出</el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script lang="ts" setup>
import { useUserInfoStore } from 'src/stores/user'
const userInfoStore = useUserInfoStore()
function onLogout () {
  userInfoStore.logout()
}
</script>

<style lang="scss" scoped></style>
