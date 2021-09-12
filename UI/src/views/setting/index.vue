<template>
  <div class="setting-container row justify-center">
    <div class="column justify-center q-gutter-sm" style="max-width: 600px">
      <div>
        <div class="text-subtitle1 q-mb-lg">
          发件间隔：
          <q-tooltip>
            单个发件箱发件连续两封邮件发件时间的间隔范围，在该范围内随机波动
          </q-tooltip>
        </div>
        <q-range
          v-model="sendInterval"
          :min="2"
          :max="20"
          :step="0.5"
          :left-label-value="sendInterval.min + '秒'"
          :right-label-value="sendInterval.max + '秒'"
          label-always
          style="min-width: 300px"
        />
      </div>
      <!-- <div>
        <div class="text-subtitle1 q-mb-lg">
          单次发件数：
          <q-tooltip> 单个发件箱单次发件总数。0代表无限制 </q-tooltip>
        </div>
        <q-slider
          v-model="singleSendControl"
          :min="0"
          :max="100"
          :step="5"
          label
          label-always
          style="min-width: 300px"
        />
      </div> -->

      <q-checkbox
        v-model="isAutoResend"
        label="自动重发"
        color="orange"
        class="self-start q-ml-xs"
      />

      <q-checkbox
        v-model="sendWithImageAndHtml"
        label="图文混发"
        color="orange"
        class="self-start q-ml-xs"
      />
    </div>
  </div>
</template>

<script>
import {
  getUserSettings,
  updateSendInterval,
  updateIsAutoResend,
  updateSendWithImageAndHtml
} from '@/api/setting'

export default {
  data() {
    return {
      sendInterval: {
        min: 3,
        max: 8
      },

      singleSendControl: 0,

      isAutoResend: true,

      sendWithImageAndHtml: false
    }
  },

  watch: {
    async sendInterval(newValue) {
      await updateSendInterval(newValue.min, newValue.max)
    },

    async isAutoResend(newValue) {
      await updateIsAutoResend(newValue)
    },

    async sendWithImageAndHtml(newValue) {
      await updateSendWithImageAndHtml(newValue)
    }
  },

  async mounted() {
    const res = await getUserSettings()
    if (!res.data) return
    const {
      sendInterval_max,
      sendInterval_min,
      isAutoResend,
      sendWithImageAndHtml
    } = res.data
    this.sendInterval.min = sendInterval_min || 3
    this.sendInterval.max = sendInterval_max || 8
    this.isAutoResend = isAutoResend
    this.sendWithImageAndHtml = sendWithImageAndHtml
  }
}
</script>

<style lang='scss'>
.setting-container {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
}
</style>