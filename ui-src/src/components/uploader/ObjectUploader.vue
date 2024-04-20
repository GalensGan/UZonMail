<template>
  <q-uploader v-bind="$attrs" ref="uploaderRef" :factory="factoryFn" @added="onFileAdded" no-thumbnails>
    <template v-slot:header="scope">
      <div class="row no-wrap items-center q-pa-sm q-gutter-xs">
        <q-btn v-if="scope.queuedFiles.length > 0" icon="clear_all" @click="scope.removeQueuedFiles" round dense flat>
          <q-tooltip>清空</q-tooltip>
        </q-btn>
        <q-btn v-if="scope.uploadedFiles.length > 0" icon="done_all" @click="scope.removeUploadedFiles" round dense
          flat>
          <q-tooltip>移除已上传文件</q-tooltip>
        </q-btn>
        <q-spinner v-if="scope.isUploading" class="q-uploader__spinner" />
        <div class="col">
          <div class="q-uploader__title">{{ label }}</div>
          <div class="q-uploader__subtitle">{{ scope.uploadSizeLabel }} / {{ scope.uploadProgressLabel }}</div>
        </div>
        <q-btn v-if="scope.canAddFiles" type="a" icon="add_box" @click="scope.pickFiles" round dense flat>
          <q-uploader-add-trigger />
          <q-tooltip>选择文件</q-tooltip>
        </q-btn>
        <!-- <q-btn v-if="scope.canUpload" icon="cloud_upload" @click="scope.upload" round dense flat>
          <q-tooltip>上传</q-tooltip>
        </q-btn> -->
        <q-btn v-if="canUpload" icon="cloud_upload" @click="scope.upload" round dense flat>
          <q-tooltip>上传</q-tooltip>
        </q-btn>
        <q-btn v-if="scope.isUploading" icon="clear" @click="scope.abort" round dense flat>
          <q-tooltip>中止上传</q-tooltip>
        </q-btn>
      </div>
    </template>

    <template v-slot:list="scope">
      <q-list dense separator>
        <q-item v-for="file in scope.files" :key="file.__key">
          <q-item-section>
            <q-item-label class="full-width ellipsis">
              {{ file.name }}
            </q-item-label>

            <!-- <q-item-label caption>
              Status: {{ file.__status }}
            </q-item-label> -->

            <q-item-label caption>
              {{ file.__sizeLabel }} / {{ file.__progressLabel }}
            </q-item-label>
          </q-item-section>

          <q-item-section v-if="file.__img" thumbnail class="gt-xs">
            <img :src="file.__img.src">
          </q-item-section>

          <q-item-section top side>
            <q-btn class="gt-xs" size="12px" flat dense round icon="delete" @click="scope.removeFile(file)" />
          </q-item-section>
        </q-item>
      </q-list>
    </template>
  </q-uploader>
</template>

<script lang="ts" setup>
defineProps({
  label: {
    type: String,
    default: ''
  }
})

import { QUploader, QUploaderFactoryObject } from 'quasar'
const uploaderRef: Ref<QUploader> = ref(null)
onMounted(() => {
  console.log('uploaderRef:', uploaderRef.value)
})

// 上传地址配置
import { useUserInfoStore } from 'src/stores/user'
const userInfoStore = useUserInfoStore()
import { FileSha256Callback, IUploadFile, fileSha256 } from 'src/utils/encrypt'
function factoryFn (files: IUploadFile[]): Promise<QUploaderFactoryObject> {
  console.log('uploader factory called:', files)
  return new Promise((resolve) => {
    // Retrieve JWT token from your store.
    const token = userInfoStore.token
    const uploadUrl = process.env.BASE_URL + '/file/upload-file-object'
    const result: QUploaderFactoryObject = {
      url: uploadUrl,
      method: 'POST',
      headers: [
        { name: 'Authorization', value: `Bearer ${token}` }
      ],
      formFields: [
        { name: 'sha256', value: files[0].__sha256 as string }
      ]
    }
    resolve(result)
  })
}

// 添加文件后的操作
import { GetFileUsageId } from 'src/api/file'
// eslint-disable-next-line @typescript-eslint/no-explicit-any
const vm = getCurrentInstance()
function sha256Callback (params: FileSha256Callback) {
  params.file.__progressLabel = params.progressLabel
  // 强制刷新
  vm?.proxy?.$forceUpdate()
}
async function onFileAdded (files: readonly IUploadFile[]) {
  // 计算文件的 sha256 值，若已经上传过，则直接修改文件状态
  console.log('files:', files)
  if (!uploaderRef.value) return

  for (const file of files) {
    // 计算 hash 值
    const sha256 = await fileSha256(file, sha256Callback)
    // 保存 hash
    file.__sha256 = sha256

    // 向服务器请求文件是否已经上传过
    const { data: fileUsageId } = await GetFileUsageId(sha256, file.name)
    if (fileUsageId < 0) continue
    // 说明文件已经上传过
    file.__fileUsageId = fileUsageId
    // 修改文件状态
    uploaderRef.value.updateFileStatus(file, 'uploaded', file.size)
    const queueIndex = uploaderRef.value.queuedFiles.findIndex(x => x.__key === file.__key)
    // 从队列中移除
    uploaderRef.value.queuedFiles.splice(queueIndex, 1)
    // 添加到已上传文件列表
    uploaderRef.value.uploadedFiles.push(file)
    // console.log('queuedFiles:', uploaderRef.value.queuedFiles)
  }
}

const canUpload = computed(() => {
  return uploaderRef.value?.queuedFiles.some(x => !x.__fileUsageId)
})
</script>

<style lang="scss" scoped></style>
