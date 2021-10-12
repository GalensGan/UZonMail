<template>
  <div class="history-container">
    <q-table
      :data="dataToShow"
      :columns="columns"
      :pagination.sync="pagination"
      row-key="_id"
      binary-state-sort
      dense
      style="height: 100%"
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

      <template v-slot:body-cell-operations="props">
        <q-td :props="props" class="row justify-end">
          <q-btn
            :size="btn_detail.size"
            :color="btn_detail.color"
            :label="btn_detail.label"
            :dense="btn_detail.dense"
            @click="openShowDetailDialog(props.row._id)"
            class="q-mr-sm"
          >
          </q-btn>
          <q-btn
            :size="btn_delete.size"
            :color="btn_delete.color"
            :label="btn_delete.label"
            :dense="btn_delete.dense"
            @click="deleteHistoryGroup(props.row._id)"
          >
          </q-btn>
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="isShowDetailDialog" persistent>
      <HistoryDetail :historyId="toShowId" @close="closeHistoryDetail" />
    </q-dialog>
  </div>
</template>

<script>
import { getHistories, deleteHistoryGroup, getHistory } from '@/api/history'
import moment from 'moment'

import { table } from '@/themes/index'
const { btn_detail, btn_delete } = table

import HistoryDetail from './components/historyDetail.vue'
import { notifyError, notifySuccess, okCancle } from '@/components/iPrompt'

export default {
  components: { HistoryDetail },
  data() {
    return {
      btn_detail,
      btn_delete,

      data: [],
      columns: [
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
          name: 'subject',
          required: true,
          label: '主题',
          align: 'left',
          field: 'subject',
          sortable: true
        },
        {
          name: 'template',
          required: true,
          label: '模板',
          align: 'left',
          field: 'templateName',
          sortable: true
        },
        {
          name: 'senderIdsLength',
          required: true,
          label: '发件箱总数',
          align: 'left',
          field: 'senderIds',
          format: val => val.length,
          sortable: true
        },
        {
          name: 'receiverIdsLength',
          required: true,
          label: '收件箱总数',
          align: 'left',
          field: 'receiverIds',
          format: val => val.length,
          sortable: true
        },
        // 状态
        {
          name: 'status',
          required: true,
          label: '状态',
          align: 'left',
          field: row => row,
          format: val => {
            const status = []

            if (val.sendStatus & 1) {
              status.push(
                `发送结束，成功：${val.successCount}/${val.receiverIds.length}`
              )
            }

            if (val.sendStatus & 2) {
              status.push(`已初始化，但未发送`)
            }

            if (val.sendStatus & 4) {
              status.push(`发送中...`)
            }

            if (val.sendStatus & 8) {
              status.push(`重发中...`)
            }

            if (val.sendStatus & 16) {
              status.push(`暂停`)
            }

            if (val.sendStatus & 32) {
              status.push(`正在发送图片`)
            }

            if (val.sendStatus & 32) {
              status.push(`正在发送html`)
            }

            return status.join()
          },
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
        rowsPerPage: 25,
        rowsNumber: 0
      },
      filter: '',

      toShowId: '',
      isShowDetailDialog: false
    }
  },

  computed: {
    dataToShow() {
      if (!this.filter) return this.data

      // 使用正则匹配，可以忽略大小写
      const regex = new RegExp(this.filter, 'i')
      return this.data.filter(d => {
        if (d.subject && regex.test(d.subject)) return true

        if (d.templateName && regex.test(d.templateName)) return true
        return false
      })
    }
  },

  async mounted() {
    // 从服务器获取历史数据
    const res = await getHistories()
    this.data = res.data
  },

  methods: {
    openShowDetailDialog(id) {
      this.toShowId = id
      this.isShowDetailDialog = true
    },

    // 删除发件
    async deleteHistoryGroup(historyId) {
      // 提醒
      const ok = await okCancle('是否删除该项发送记录？')
      if (!ok) return

      // 开始删除
      await deleteHistoryGroup(historyId)

      // 删除现有数据
      const index = this.data.findIndex(d => d._id === historyId)
      if (index > -1) {
        this.data.splice(index, 1)
      }

      // 提示成功
      notifySuccess('删除成功')
    },

    // 关闭详细面板
    async closeHistoryDetail(historyId) {
      // 从服务器拉取该条数据
      const res = await getHistory(historyId)
      if (!res.data) {
        this.isShowDetailDialog = false
        notifyError('更新历史记录失败')
        return
      }

      const index = this.data.findIndex(d => d._id === historyId)
      if (index > -1) this.data.splice(index, 1, res.data)

      this.isShowDetailDialog = false
    }
  }
}
</script>

<style lang="scss">
.history-container {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
}
</style>