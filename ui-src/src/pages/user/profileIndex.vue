<template>
  <q-card>
    <q-tabs v-model="tabModel" dense class="text-grey" active-color="primary" indicator-color="primary" align="justify">
      <q-tab name="userInfo" label="用户信息" />
      <q-tab name="changePassword" label="修改密码" />
    </q-tabs>

    <q-separator />
    <q-tab-panels v-model="tabModel" animated>
      <q-tab-panel name="userInfo" class="q-pa-none">

      </q-tab-panel>

      <q-tab-panel name="changePassword">

      </q-tab-panel>
    </q-tab-panels>
  </q-card>
</template>

<script lang="ts" setup>
const tabModel = ref('mails')
</script>

<style lang="scss" scoped></style>
