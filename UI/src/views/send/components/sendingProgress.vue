<template>
  <q-card class="q-ma-md" style="width: 400px">
    <div class="q-pa-sm">
      正在发送：{{ sendingData.receverEmail }}, {{ sendingData.index }} /
      {{ sendingData.total }}
    </div>
    <q-linear-progress
      size="30px"
      rounded
      :value="getProgressValue()"
      color="orange"
    >
      <div class="absolute-full flex flex-center">
        <q-badge
          color="white"
          text-color="accent"
          :label="getProgressLable()"
        />
      </div>
    </q-linear-progress>
  </q-card>
</template>

<script>
import { getSendingInfo } from '@/api/send'
export default {
  data() {
    return {
      sendingData: {
        index: 0,
        total: 1
      }
    }
  },

  async mounted() {
    const res = await getSendingInfo()
    this.sendingData = res.data
    if (this.sendingData.index < this.sendingData.total) {
      this.getProgressInfo()
    }
  },

  methods: {
    getProgressValue() {
      return this.sendingData.index / this.sendingData.total
    },

    getProgressLable() {
      return (
        ((this.sendingData.index * 100) / this.sendingData.total).toFixed(1) +
        ' %'
      )
    },

    getProgressInfo() {
      setTimeout(async () => {
        // 获取更新数据
        const res = await getSendingInfo()
        console.log('getProgressInfo:', res.data)
        this.sendingData = res.data
        if (this.sendingData.index < this.sendingData.total) {
          this.getProgressInfo()
        } else {
          this.$emit('close')
        }
      }, 500)
    }
  }
}
</script>

<style>
</style>