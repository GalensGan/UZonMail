<template>
  <q-menu context-menu auto-close transition-show="scale" transition-hide="scale">
    <q-list style="min-width: 100px" dense>
      <q-item v-for="item in contextItems" :key="item.name" clickable v-close-popup @click="item.onClick"
        :class="getItemClass(item)">
        <q-item-section>{{ item.label }}
        </q-item-section>
      </q-item>
    </q-list>
  </q-menu>
</template>

<script lang="ts" setup>
import { IContextMenuItem } from './types'
const props = defineProps({
  items: {
    type: Array as PropType<IContextMenuItem[]>,
    required: true
  },
  value: {
    type: Object,
    required: true
  }
})

// 对定义进行处理
const colors = ['primary', 'secondary', 'accent']
const contextItems = computed(() => {
  // 如果有 vif,则执行 vif 判断是否显示
  const results = props.items.filter(x => !x.vif || x.vif(props.value))
  // 对结果赋予颜色
  results.forEach((item, index) => {
    if (item.color) return

    // 若没有颜色，则挑选与前后不一样的颜色
    const colorsTemp: (string | undefined)[] = []
    if (index > 0) colorsTemp.push(results[index - 1].color)
    if (index < results.length - 1) colorsTemp.push(results[index + 1].color)
    item.color = colors.find(x => !colorsTemp.includes(x))
  })
  return results
})
function getItemClass (contextItem: IContextMenuItem) {
  return {
    'text-primary': contextItem.color === 'primary',
    'text-secondary': contextItem.color === 'secondary',
    'text-accent': contextItem.color === 'accent'
  }
}
</script>

<style lang="scss" scoped></style>
