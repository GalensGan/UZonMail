<template>
  <div :id="id" :class="className" :style="{ height: height, width: width }" />
</template>

<script>
import 'echarts-liquidfill'
import * as echarts from 'echarts'
import resize from '@/components/Echarts/mixins/resize'
import _ from 'lodash'
import { getInboxCountOfTyes } from '@/api/report'

export default {
  mixins: [resize],
  props: {
    className: {
      type: String,
      default: 'chart'
    },
    id: {
      type: String,
      default: 'inboxType'
    },
    width: {
      type: String,
      default: '400px'
    },
    height: {
      type: String,
      default: '200px'
    }
  },
  data() {
    return {
      chart: null,
      source: []
    }
  },

  async mounted() {
    // 获取所有收件箱的种类和数量
    const res = await getInboxCountOfTyes()
    this.source = res.data

    this.initChart()
  },
  beforeDestroy() {
    if (!this.chart) {
      return
    }
    this.chart.dispose()
    this.chart = null
  },
  methods: {
    initChart() {
      // 参数
      const option = {
        legend: {
          right: '0',
          orient: 'vertical'
        },
        tooltip: {
          trigger: 'item'
        },

        dataset: {
          // source: [
          //   { value: 40, name: 'rose 1' },
          //   { value: 38, name: 'rose 2' },
          //   { value: 32, name: 'rose 3' },
          //   { value: 30, name: 'rose 4' },
          //   { value: 28, name: 'rose 5' },
          //   { value: 26, name: 'rose 6' },
          //   { value: 22, name: 'rose 7' },
          //   { value: 18, name: 'rose 8' }
          // ]

          source: this.source
        },
        series: [
          {
            name: '收件数量统计',
            type: 'pie',
            radius: [60, 94],
            center: ['50%', '50%'],
            roseType: 'area',
            itemStyle: {
              borderRadius: 8
            },
            color: [
              '#ff915a',
              '#9fe080',
              '#7ed3f4',
              '#ff7070',
              '#ffdc60',
              '#3ba272',
              '#5c7bd9',
              '#a969c6'
            ]
          }
        ]
      }

      // 基于准备好的dom，初始化echarts实例
      var myChart = echarts.init(document.getElementById(this.id))
      // 绘制图表
      myChart.setOption(option)

      // 监听事件
    }
  }
}
</script>

<style>
</style>
