<template>
  <q-field v-model="fieldModelValue" tag="div" label="数据" dense @focus="isActive = true" @blur="isActive = false">
    <template v-slot:before>
      <q-icon name="analytics" color="primary" />
    </template>

    <template v-slot:control>
      <div class="full-width no-outline">
        <div class="text-grey-7"> {{ fieldText }}</div>
      </div>
    </template>

    <template v-slot:append>
      <div class="row justify-end">
        <q-btn v-if="fieldModelValue" round dense flat icon="close" color="negative"
          @click.prevent="onRemoveSelectedFile">
          <q-tooltip>
            删除当前数据
          </q-tooltip>
        </q-btn>

        <q-btn v-if="isActive" round dense flat icon="article" color="secondary" class="q-ml-sm"
          @click.prevent="onDownloadEmailDataTemplate">
          <q-tooltip>
            下载模板
          </q-tooltip>
        </q-btn>

        <q-btn round dense flat icon="add" class="q-ml-sm" @click.prevent="onSelectExcel" color="grey-7">
          <q-tooltip>
            选择数据
          </q-tooltip>
        </q-btn>
      </div>
    </template>
  </q-field>
</template>

<script lang="ts" setup>
// 模板数据
const modelValue = defineModel({
  type: Array,
  default: () => []
})

// 选择模板数据
import { readExcelCore } from 'src/utils/file'
async function onSelectExcel () {
  const { files, sheetName, data } = await readExcelCore({
    sheetIndex: 0,
    selectSheet: true
  })
  if (files) {
    const firstFile = files[0]
    fieldModelValue.value = `${firstFile.name} - ${sheetName}`
  }
  modelValue.value = data
}

// placeholder 显示
import { useCustomQField } from '../helper'
const { isActive, fieldModelValue, fieldText } = useCustomQField('请选择数据 (该项可为空)')

// 删除选择的数据
function onRemoveSelectedFile () {
  fieldModelValue.value = ''
  modelValue.value = []
}

// 下载模板
function onDownloadEmailDataTemplate () {

}
</script>

<style lang="scss" scoped></style>
