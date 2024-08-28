<template>
  <q-list class="basic-settings-container">
    <SendSetting />
    <OrganizationSetting />
    <OrganizationSendingSetting />
  </q-list>
</template>

<script lang="ts" setup>
import SendSetting from './expansionItems/SendSetting.vue'
import OrganizationSendingSetting from './expansionItems/OrganizationSendingSetting.vue'
import OrganizationSetting from './expansionItems/OrganizationSetting.vue'

</script>

<style lang="scss" scoped></style>

<style lang="scss">
.basic-settings-container {
  .q-expansion-item {
    padding-bottom: 6px;

    .q-expansion-item__container {
      border-radius: 6px;
      background-color: white;

      .q-item {
        background-color: unset;
      }
    }
  }
}
</style>
