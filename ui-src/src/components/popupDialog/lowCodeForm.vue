<template>
  <q-dialog ref="dialogRef" @hide="onDialogHide" :persist="persist">
    <q-card class="q-dialog-plugin">
      <!--
        ... 内容
        ... 用q-card-section来做？
      -->
      <div class="q-py-md q-px-xs row justify-start items-center low-code__container">
        <template v-for="field in formattedFields" :key="field.name">
          <q-input v-if="isMatchedType(field, 'text')" outlined class="q-mb-sm low-code__field q-px-sm" standout dense
            v-model="results[field.name]" :label="field.label">
            <template v-if="field.icon" v-slot:prepend>
              <q-icon :name="field.icon" />
            </template>
          </q-input>
        </template>
      </div>

      <!-- 按钮的例子 -->
      <q-card-actions align="right">
        <OkBtn @click="onOKClick"></OkBtn>
        <CancelBtn @click="onDialogCancel"></CancelBtn>
      </q-card-actions>
    </q-card>
  </q-dialog>
</template>

<script lang="ts" setup>
import { useDialogPluginComponent } from 'quasar'
import { IPopupDialogParams } from './types'

import OkBtn from 'src/components/componentWrapper/buttons/OkBtn.vue'
import CancelBtn from 'src/components/componentWrapper/buttons/CancelBtn.vue'

const props = defineProps<IPopupDialogParams>()

// 是否为匹配到的类型
function isMatchedType (field: object, type: string): boolean {
  return field.type === type
}

// 获取数据源
const { fields, dataSet } = toRefs(props)
const dataSetResults = ref({})
const results = ref({})
onMounted(async () => {
  // 计算
  Object.keys(dataSet.value).forEach(key => {
    dataSetResults.value[key] = dataSet.value[key]
  })
})

// 格式化后的字段
const formattedFields = computed(() => {
  const results = []
  for (const field of fields.value) {
    results.push(field)
  }

  return results
})

// #region quasar 弹窗逻辑
defineEmits([
  // 必需；需要指定一些事件
  // （组件将通过useDialogPluginComponent()发出）
  ...useDialogPluginComponent.emits
])

const { dialogRef, onDialogHide, onDialogOK, onDialogCancel } = useDialogPluginComponent()
// dialogRef - 应用于QDialog的Vue引用。
// onDialogHide - 用作QDialog上@hide的处理函数。
// onDialogOK - 调用函数来处理结果为"确定"的对话框。
//              例子: onDialogOK() - 没有有效载荷
//              例子: onDialogOK({ /*...*/ }) -- 有有效载荷
// onDialogCancel - 调用函数来处理结果为"取消"的对话框。

// 这是我们例子的一部分（所以不是必须的）。
function onOKClick () {
  // 在"确定"时，它必须要
  // 调用onDialogOK（带可选的有效载荷）。
  onDialogOK()
  // 或使用有效载荷：onDialogOK({ ... })
  // ...它还会自动隐藏对话框
}
// #endregion
</script>

<style lang="scss" scoped>
.low-code__container {
  display: flex;
  flex-wrap: wrap;

  .low-code__field {
    flex: 1 1 100%;

    @media (min-width: 600px) {
      flex: 1 1 50%;
    }
  }
}
</style>
