<template>
  <q-table
    :data="data"
    :columns="columns"
    row-key="id"
    :pagination.sync="pagination"
    :loading="loading"
    :filter="filter"
    binary-state-sort
    dense
    class="emails-table"
  >
    <template v-slot:top>
      <div class="row justify-center q-gutter-sm">        
        <q-btn label="新增" dense size="sm" outline color="secondary"></q-btn>
        <q-btn label="批量新增" dense size="sm" outline color="orange"></q-btn>
        <span class="text-subtitle1 text-primary">{{ group.name }}</span>
      </div>
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
    <template v-slot:body-cell-operation="props">
      <q-td :props="props">
        <q-btn
          :size="btn_modify.size"
          :color="btn_modify.color"
          :label="btn_modify.label"
          :dense="btn_modify.dense"
          @click="modifyUserInfo(props.row.userId)"
        >
          <q-tooltip>转到用户信息编辑界面</q-tooltip>
        </q-btn>
      </q-td>
    </template>
  </q-table>
</template>

<script>
export default {
  props: {
    group: {
      type: Object,
      default() {
        return {
          groupType: 'send'
        }
      }
    }
  },

  computed: {
    columns() {
      if (this.group.groupType === 'send') {
        return [
          {
            name: 'userName',
            required: true,
            label: '姓名',
            align: 'left',
            field: row => row.useName,
            sortable: true
          },
          {
            name: 'email',
            required: true,
            label: '邮箱',
            align: 'left',
            field: row => row.email,
            sortable: true
          },
          {
            name: 'smtp',
            required: true,
            label: 'SMTP服务器地址',
            align: 'left',
            field: row => row.smtp,
            sortable: true
          },
          {
            name: 'password',
            required: true,
            label: 'SMTP密码',
            align: 'left',
            field: row => row.password,
            sortable: true
          }
        ]
      } else {
        return [
          {
            name: 'userName',
            required: true,
            label: '姓名',
            align: 'left',
            field: row => row.useName,
            sortable: true
          },
          {
            name: 'email',
            required: true,
            label: '邮箱',
            align: 'left',
            field: row => row.email,
            sortable: true
          }
        ]
      }
    }
  },

  data() {
    return {
      filter: '',
      loading: false,
      // 分页数据
      pagination: {
        sortBy: 'userId',
        descending: false,
        page: 1,
        rowsPerPage: 15,
        rowsNumber: 0
      },

      data: []
    }
  },

  async mounted() {
    // 根据 groupId 找到邮箱
  }
}
</script>

<style lang='scss'>
.emails-table {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
}
</style>