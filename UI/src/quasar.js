import Vue from 'vue'

import './styles/quasar.scss'
import lang from 'quasar/lang/zh-hans.js'
import '@quasar/extras/material-icons/material-icons.css'
import { Quasar, Notify, Dialog } from 'quasar'

Vue.use(Quasar, {
  components: {
    /* not needed if importStrategy is not 'manual' */
  },
  directives: {
    /* not needed if importStrategy is not 'manual' */
  },
  plugins: {
    Notify,
    Dialog
  },
  lang: lang,
  config: {
    notify: {
      position: 'top'
    }
  }
})
