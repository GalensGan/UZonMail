<template>
  <div class="dashboard-container">
    <q-img
      src="http://5b0988e595225.cdn.sohucs.com/images/20200308/ae911756f12c4023b9d61983c9791471.jpeg"
      class="full-height"
    >
      <div class="absolute-full text-h2 flex flex-center welcome-word">
        <div>批量发送，如此简单！</div>
      </div>
    </q-img>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import live2d from '../login/mixins/live2d.vue'

export default {
  name: 'Dashboard',
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
