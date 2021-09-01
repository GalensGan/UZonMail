<template>
  <q-card class="column" style="max-width: none; height: 60%">
    <q-table
      :data="dataToShow"
      :columns="columns"
      row-key="_id"
      binary-state-sort
      :pagination.sync="pagination"
      dense
      style="flex: 1"
    >
      <template v-slot:top>
        <q-space />
        <q-input
          dense
          debounce="300"
          placeholder="搜索"
          color="primary"
          v-model="filter"
        >
          <template v-slot:append>
            <q-icon name="search" />
          </template>
        </q-input>
      </template>

      <template v-slot:body-cell-index="props">
        <q-td :props="props" class="row justify-end">
          {{ props.rowIndex + 1 }}
        </q-td>
      </template>

      <template v-slot:body-cell-operations="props">
        <q-td :props="props" class="row justify-end">
          <q-btn
            :size="btn_detail.size"
            :color="btn_detail.color"
            :label="btn_detail.label"
            :dense="btn_detail.dense"
            @click="openShowDetailDialog(props.row._id)"
          >
          </q-btn>
        </q-td>
      </template>
    </q-table>
    <div class="row justify-end q-pa-sm">
      <q-btn
        :label="resendLabel"
        color="teal"
        size="sm"
        class="q-mr-sm"
        v-if="isShowResent"
        :disable="disableResend"
        :loading="isResending"
        @click="resend"
      />

      <q-btn
        :disable="disableCancle"
        label="取消"
        color="negative"
        size="sm"
        @click="closeHistoryDetail"
      />
    </div>
  </q-card>
</template>

<script>
import {
  getSendItemsByHistoryId,
  getHistoryGroupSendResult
} from '@/api/history'

import { resendFail, getCurrentStatus, getSendingInfo } from '@/api/send'

import { table } from '@/themes/index'
const { btn_detail } = table

import moment from 'moment'
import { notifyError, notifySuccess, okCancle } from '@/components/iPrompt'

export default {
  props: {
    historyId: {
      type: String,
      default: ''
    }
  },
  data() {
    return {
      btn_detail,

      filter: '',
      data: [],
      columns: [
        {
          name: 'index',
          required: true,
          label: '序号',
          align: 'left'
        },
        {
          name: 'createDate',
          required: true,
          label: '日期',
          align: 'left',
          field: 'createDate',
          format: val => moment(val).format('YYYY-MM-DD'),
          sortable: true
        },
        {
          name: 'senderName',
          required: true,
          label: '发件人',
          align: 'left',
          field: 'senderName',
          sortable: true
        },
        {
          name: 'senderEmail',
          required: true,
          label: '发件箱',
          align: 'left',
          field: 'senderEmail',
          sortable: true
        },
        {
          name: 'receiverName',
          required: true,
          label: '收件人',
          align: 'left',
          field: 'receiverName',
          sortable: true
        },
        {
          name: 'receiverEmail',
          required: true,
          label: '收件箱',
          align: 'left',
          field: 'receiverEmail',
          sortable: true
        },
        // 状态
        {
          name: 'isSent',
          required: true,
          label: '状态',
          align: 'left',
          field: 'isSent',
          format: val => (val ? '成功' : '失败'),
          sortable: true
        },
        {
          name: 'sendMessage',
          required: true,
          label: '原因',
          align: 'left',
          field: 'sendMessage',
          sortable: true
        },
        {
          name: 'tryCount',
          required: true,
          label: '重发次数',
          align: 'left',
          field: 'tryCount',
          sortable: true
        },
        {
          name: 'operations',
          required: true,
          label: '操作',
          align: 'right'
        }
      ],
      // 分页数据
      pagination: {
        sortBy: '_id',
        descending: false,
        page: 1,
        rowsPerPage: 0,
        rowsNumber: 0
      },

      disableResend: false,
      disableCancle: false,
      isResending: false,
      resendLabel: '重发'
    }
  },
  computed: {
    dataToShow() {
      if (!this.filter) return this.data

      return this.data.filter(d => {
        if (d.subject && d.subject.indexOf(this.filter) > -1) return true
        return false
      })
    },

    // 是否显示重新发送功能
    isShowResent() {
      const needResend = this.data.filter(d => !d.isSent)
      return needResend.length > 0
    }
  },
  async mounted() {
    // 获取数据
    if (!this.historyId) return

    const res = await getSendItemsByHistoryId(this.historyId)
    this.data = res.data

    // 查看是否可以重新发送
    const needResend = this.data.filter(d => !d.isSent)
    this.disableResend = needResend.length < 1

    // 获取当前状态，可能处理发送中
    const statusRes = await getCurrentStatus()
    if (statusRes.data > 1) {
      // 打开发送框
      this.disableResend = true
      this.disableCancle = true
      this.isResending = true

      this.getProgressInfo()
    }
  },

  methods: {
    async resend() {
      const ids = this.data.filter(d => !d.isSent).map(d => d._id)

      // 提示
      const ok = await okCancle(`是否重发所有失败项?共 ${ids.length} 项。`)
      if (!ok) return

      if (!ids || ids.length < 1) {
        notifyError('未找到可发送项')
        return
      }

      // 关闭重发
      this.disableResend = true
      this.isResending = true

      // 开始发送
      await resendFail(this.historyId, ids)

      this.getProgressInfo()
    },

    // 轮询获取进度
    getProgressInfo() {
      setTimeout(async () => {
        // 获取更新数据
        const res = await getSendingInfo()
        // console.log('getProgressInfo:', res.data)
        this.sendingInfo = res.data
        if (this.sendingInfo.index < this.sendingInfo.total) {
          this.getProgressInfo()
        } else {
          // 获取发送结果
          const msgRes = await getHistoryGroupSendResult(
            this.sendingInfo.historyId
          )

          if (msgRes.data.ok) notifySuccess(msgRes.data.message)
          else notifyError(msgRes.data.message)

          // 更新数据
          const res = await getSendItemsByHistoryId(this.historyId)
          this.data = res.data

          const needResend = this.data.filter(d => !d.isSent)
          this.disableResend = needResend.length < 1

          this.disableCancle = false
          this.isResending = false
        }
      }, 800)
    },

    // 关掉窗口后，主程序需要更新当前数据
    closeHistoryDetail() {
      this.$emit('close', this.historyId)
    }
  }
}
</script>

<style>
</style>