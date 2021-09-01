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

export default {
  name: 'Dashboard',
  computed: {
    ...mapGetters(['name'])
  },
  mounted() {
    const waifu = document.getElementById('waifu')
    if (!waifu) {
      this.loadJs()
      this.loadCss()
    }
  },
  methods: {
    loadJs() {
      const s = document.createElement('script')
      s.type = 'text/javascript'
      s.src =
        'https://cdn.jsdelivr.net/gh/stevenjoezhang/live2d-widget@latest/autoload.js'
      document.body.appendChild(s)
    },
    loadCss() {
      const s = document.createElement('link')
      s.rel = 'stylesheet'
      s.href =
        'https://cdn.jsdelivr.net/npm/font-awesome/css/font-awesome.min.css'
      document.body.appendChild(s)
    }
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
