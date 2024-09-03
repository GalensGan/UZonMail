<template>
  <q-item style="height: 100px;" class="animated slideInDown">
    <q-item-section avatar class="q-pr-none">
      <q-icon :name="resolveSvgFullName('undraw_mailbox_re_dvds')" size="70px" class="q-ma-lg"></q-icon>
    </q-item-section>

    <q-item-section>
      <h6>宇正群邮</h6>
    </q-item-section>
  </q-item>
</template>

<script lang="ts" setup>
import { resolveSvgFullName } from 'src/utils/svgHelper'
</script>

<style lang="scss" scoped></style>
