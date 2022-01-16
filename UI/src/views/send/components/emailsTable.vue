<template>
  <q-table
    :data="dataToShow"
    :columns="columns"
    row-key="_id"
    binary-state-sort
    dense
    selection="multiple"
    :selected.sync="selected"
    style="height: 100%"
  >
    <template v-slot:top>
      <q-space />
      <q-input
        v-model="filter"
        dense
        debounce="300"
        placeholder="搜索"
        color="primary"
      >
        <template v-slot:append>
          <q-icon name="search" />
        </template>
      </q-input>
    </template>
  </q-table>
</template>

<script>
import { getEmails } from '@/api/group'

export default {
  props: {
    group: {
      type: Object,
      default() {
        return {
          groupType: 'send'
        }
      }
    },
    value: {
      type: Array,
      default() {
        return []
      }
    }
  },
  data() {
    return {
      filter: '',
      data: [],
      selected: this.value
    }
  },

  computed: {
    dataToShow() {
      if (!this.filter) return this.data

      return this.data.filter(d => {
        if (d.userName && d.userName.indexOf(this.filter) > -1) return true
        if (d.email && d.email.indexOf(this.filter) > -1) return true
        if (d.smtp && d.smtp.indexOf(this.filter) > -1) return true
        if (d.password && d.password.indexOf(this.filter) > -1) return true
        return false
      })
    },

    columns() {
      return [
        {
          name: 'userName',
          required: true,
          label: '姓名',
          align: 'left',
          field: row => row.userName,
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
      // if (this.group.groupType === 'send') {
      //   return [
      //     {
      //       name: 'userName',
      //       required: true,
      //       label: '姓名',
      //       align: 'left',
      //       field: row => row.userName,
      //       sortable: true
      //     },
      //     {
      //       name: 'email',
      //       required: true,
      //       label: '邮箱',
      //       align: 'left',
      //       field: row => row.email,
      //       sortable: true
      //     },
      //     {
      //       name: 'smtp',
      //       required: true,
      //       label: 'SMTP服务器地址',
      //       align: 'left',
      //       field: row => row.smtp,
      //       sortable: true
      //     },
      //     {
      //       name: 'password',
      //       required: true,
      //       label: 'SMTP密码',
      //       align: 'left',
      //       field: row => row.password,
      //       sortable: true
      //     },
      //     {
      //       name: 'operation',
      //       label: '操作',
      //       align: 'right'
      //     }
      //   ]
      // } else {
      //   return [
      //     {
      //       name: 'userName',
      //       required: true,
      //       label: '姓名',
      //       align: 'left',
      //       field: row => row.userName,
      //       sortable: true
      //     },
      //     {
      //       name: 'email',
      //       required: true,
      //       label: '邮箱',
      //       align: 'left',
      //       field: row => row.email,
      //       sortable: true
      //     },
      //     {
      //       name: 'operation',
      //       label: '操作',
      //       align: 'right'
      //     }
      //   ]
      // }
    }
  },

  watch: {
    selected(val) {
      console.log('selected data:', val)
      const results = val.map(u => {
        return {
          type: 'email',
          _id: u._id,
          label: u.userName
        }
      })
      this.$emit('input', results)
    }
  },

  async mounted() {
    const { data } = await getEmails(this.group._id)
    this.data = data || []

    // 修改选择的数据
    this.selected = this.data.filter(
      d => this.selected.findIndex(s => s._id === d._id) > -1
    )
  }
}
</script>

<style>
</style>
