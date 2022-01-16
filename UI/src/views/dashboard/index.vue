<template>
  <div class="dashboard-container column justify-center q-ma-lg">
    <div class="row justify-center">
      <SuccessfullRate />
      <InboxTypes />
    </div>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import live2d from '../login/mixins/live2d.vue'

import SuccessfullRate from './components/successfullRate.vue'
import InboxTypes from './components/inboxTypes'

export default {
  name: 'Dashboard',
  components: { SuccessfullRate, InboxTypes },
  mixins: [live2d],
  computed: {
    ...mapGetters(['name'])
  }
}
</script>

<style lang="scss" scoped>
.dashboard-container {
  position: absolute;
  left: 0;
  top: 0;
  right: 0;
  bottom: 0;

  .welcome-word {
    font-family: cursive;
    color: aliceblue;
  }
}
</style>
