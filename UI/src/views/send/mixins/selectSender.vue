<script>
export default {
  data() {
    return {
      // 发件箱
      senders: [],
      isShowSelectedSenders: false
    }
  },

  methods: {
    // 打开发件人选择框
    openSelectSendersDialog() {
      this.isShowSelectedSenders = true
    },

    // 移除发件人
    removeSender(sender) {
      const index = this.senders.findIndex(
        re => re.type === sender.type && re._id === sender._id
      )
      if (index > -1) this.senders.splice(index, 1)
    }
  }
}
</script>