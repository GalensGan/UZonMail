/* eslint-disable vue/no-async-in-computed-properties */

<script>
// 快速导入表格组件配置
// 使用需知：
// 1. 在data中已经定义了如下属性，请不要在组件中重复定义用作它用
export default {
  data() {
    return {
      filter: '',
      loading: false,
      pagination: {
        sortBy: 'order',
        descending: true,
        page: 1,
        rowsPerPage: 15,
        rowsNumber: 0
      },
      data: []
    }
  },

  computed: {
    // 调用方法来生成过滤类
    async initQuasarTable_computed_filterObj() {
      console.log('initQuasarTable_computed_filterObj:', this.filter)

      // 返回 promise
      return await this.initQuasarTable_createFilter(this.filter)
    },

    // 调用方法来获取数据
    async initQuasarTable_computed_rowsNumber() {
      const filterObj = await this.initQuasarTable_computed_filterObj

      const result = await this.initQuasarTable_getFilterCount(filterObj)

      console.log('initQuasarTable_rowsNumber:', result)
      // 赋值
      return result
    }
  },

  // 缓存激活后更新数据
  async activated() {
    this.initQuasarTable_onRequest({
      pagination: this.pagination
    })
  },

  // mixin 中的mounted会先于组件执行
  async mounted() {
    // 初始化数据
    this.initQuasarTable_onRequest({
      pagination: this.pagination
    })
  },

  // mixin 中的方法会被组件中的同名方法覆盖
  // 前面三个方法需要在组件中自定义
  methods: {
    // 获取筛选数量
    // 在组件内需要覆盖该方法
    // 该方法必须返回一个数值
    async initQuasarTable_getFilterCount(filterObj) {
      return 0
    },

    // 获取筛选结果
    async initQuasarTable_getFilterList(filterObj, pagination) {
      return []
    },

    // 对返回数据的处理
    async initQuasarTable_resultHandler(data) {
      return data
    },

    // 生成过滤条件,返回一个对象
    async initQuasarTable_createFilter(filter) {
      return {
        filter
      }
    },

    // 请求数据的方法
    async initQuasarTable_onRequest(props) {
      let paginationParams = this.pagination
      if (props) {
        paginationParams = props.pagination
      }

      // 如果没有选中，则不显示
      const { page, rowsPerPage, sortBy, descending } = paginationParams

      this.loading = true

      console.log('initQuasarTable_onRequest:0')

      // update rowsCount with appropriate value
      const rowsCount = await this.initQuasarTable_computed_rowsNumber

      console.log('initQuasarTable_onRequest:1')

      // get all rows if "All" (0) is selected
      const fetchCount = rowsPerPage === 0 ? rowsCount : rowsPerPage

      // 为0时直接返回空
      if (fetchCount === 0) {
        this.loading = false
        this.data = []
        return
      }

      // calculate starting row of data
      const startRow = (page - 1) * rowsPerPage

      // 创建过滤条件
      const filterObj = await this.initQuasarTable_computed_filterObj

      const pagination = {
        sortBy,
        descending,
        skip: startRow,
        limit: fetchCount
      }

      // fetch data from "server"
      const returnedData = await this.initQuasarTable_getFilterList(
        filterObj,
        pagination
      )

      // clear out existing data and add new
      // 对数据进行处理
      this.data = await this.initQuasarTable_resultHandler(returnedData)

      // don't forget to update local pagination object
      this.pagination.page = page
      this.pagination.rowsPerPage = rowsPerPage
      this.pagination.sortBy = sortBy
      this.pagination.descending = descending
      this.pagination.rowsNumber = rowsCount

      // ...and turn of loading indicator
      this.loading = false
    }
  }
}
</script>
