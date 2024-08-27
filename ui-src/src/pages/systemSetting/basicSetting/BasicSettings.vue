<template>
  <q-list class="basic-settings-container">
    <SendSetting />
  </q-list>
</template>

<script lang="ts" setup>
import SendSetting from './expansionItems/SendSetting.vue'
</script>

<style lang="scss" scoped></style>

<style lang="scss">
.basic-settings-container {
  .q-expansion-item__container {
    border-radius: 6px;
    background-color: white;
  }
}
</style>
