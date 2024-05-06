<template>
  <q-linear-progress v-bind="$attrs" :value="progressValue" class="uzon-linear-progress" color="secondary"
    :style="styleObject">
    <div class="absolute-full flex flex-center">
      <q-badge color="transparent" text-color="accent" :label="progressLabel" />
    </div>
  </q-linear-progress>
</template>

<script lang="ts" setup>
const props = defineProps({
  progressValue: {
    type: Number,
    default: 0
  },

  // 进度条宽度，单位 px
  width: {
    type: Number,
    default: 400
  },

  borderRadius: {
    type: Number,
    default: 4
  }
})
const { progressValue } = toRefs(props)
const progressLabel = computed(() => `${(progressValue.value * 100).toFixed(2)}%`)
// 当屏幕宽度大于 width 时，取 width，否则取 100%
const styleObject = computed(() => ({ width: `min(${props.width}px, 100vw)`, borderRadius: `${props.borderRadius}px` }))
</script>

<style lang="scss" scoped></style>
