<script>
export default {
  data() {
    return {
      // 收件箱
      receivers: [],
      isShowSelectEmails: false
    }
  },

  methods:{
      openSelectReceiversDialog() {
      this.isShowSelectEmails = true
    },

    removeReceiver(receiver) {
      const index = this.receivers.findIndex(
        re => re.type === receiver.type && re._id === receiver._id
      )
      if (index > -1) this.receivers.splice(index, 1)
    }
  }
}
</script>