<template>
  <q-tooltip ref="tooltipRef" class="bg-primary" :anchor="anchor" :self="self" transition-show="rotate"
    transition-hide="rotate" @before-show="onTooltipBeforeShow" v-model="tooltipModel" max-width="40em">
    <div v-for="tip in tooltips" :key="tip">{{ tip }}</div>
  </q-tooltip>
</template>

<script lang="ts" setup>
import { QTooltip } from 'quasar'
import { PropType } from 'vue'

/**
 * 说明
 * 该组件是一个异步组件，可以同步使用，仅在需要时，才会请求数据
 */

const props = defineProps({
  anchor: {
    type: String as PropType<'top left'
      | 'top middle'
      | 'top right'
      | 'top start'
      | 'top end'
      | 'center left'
      | 'center middle'
      | 'center right'
      | 'center start'
      | 'center end'
      | 'bottom left'
      | 'bottom middle'
      | 'bottom right'
      | 'bottom start'
      | 'bottom end'
      | undefined>,
    default: undefined
  },

  self: {
    type: String as PropType<'top left'
      | 'top middle'
      | 'top right'
      | 'top start'
      | 'top end'
      | 'center left'
      | 'center middle'
      | 'center right'
      | 'center start'
      | 'center end'
      | 'bottom left'
      | 'bottom middle'
      | 'bottom right'
      | 'bottom start'
      | 'bottom end'
      | undefined>,
    default: undefined
  },

  // 参数
  params: {
    type: Object,
    default: () => { return {} }
  },
  // 是否缓存
  cache: {
    type: Boolean,
    default: true
  },
  // 可以是字符串，字符串数组，或者是一个返回字符串数组的函数
  tooltip: {
    type: [Array, Function, String] as PropType<Array<string> | ((params?: object) => Promise<string[]>) | string>,
    default: () => []
  }
})

const tooltips: Ref<string[]> = ref([])
let cached = false
const tooltipModel = ref(false)
async function onTooltipBeforeShow () {
  const { params, cache, tooltip } = toRefs(props)
  if (cache && cached) {
    if (tooltips.value.length === 0) {
      tooltipModel.value = false
    }
    return
  }
  cached = true

  const tooltipsResult: string[] = []
  if (typeof tooltip.value === 'string') {
    tooltipsResult.push(tooltip.value)
  } else if (Array.isArray(tooltip.value)) {
    tooltipsResult.push(...tooltip.value)
  } else if (typeof tooltip.value === 'function') {
    const tipItems = await tooltip.value(params.value)
    tooltipsResult.push(...tipItems)
  }

  // console.log('tooltipsResult', tooltipsResult)
  // 过滤掉空值
  tooltips.value = tooltipsResult.filter(item => item)
  // console.log('tooltips is empty', tooltips.value)
  if (!tooltip.value || tooltip.value.length === 0) {
    // 隐藏不显示
    tooltipModel.value = false
  }
}
</script>
<style lang="scss" scoped></style>
