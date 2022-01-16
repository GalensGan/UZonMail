import _ from 'lodash'

export default {
  data() {
    return {
      $_sidebarElm: null
    }
  },
  mounted() {
    // 每隔100ms调用一次
    this.__resizeHandler = _.debounce(() => {
      if (this.chart) {
        this.chart.resize()
      }
    }, 100)
    window.addEventListener('resize', this.__resizeHandler)

    this.$_sidebarElm = document.getElementsByClassName('sidebar-container')[0]
    this.$_sidebarElm &&
      this.$_sidebarElm.addEventListener(
        'transitionend',
        this.$_sidebarResizeHandler
      )
  },

  beforeDestroy() {
    window.removeEventListener('resize', this.__resizeHandler)

    this.$_sidebarElm &&
      this.$_sidebarElm.removeEventListener(
        'transitionend',
        this.$_sidebarResizeHandler
      )
  },
  methods: {
    // use $_ for mixins properties
    // https://vuejs.org/v2/style-guide/index.html#Private-property-names-essential
    $_sidebarResizeHandler(e) {
      if (e.propertyName === 'width') {
        this.__resizeHandler()
      }
    }
  }
}
