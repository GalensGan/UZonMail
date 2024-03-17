<template>
  <q-layout view="lHh Lpr lFf">
    <q-header class="bg-grey">
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />
        <q-toolbar-title>
          <BreadcrumbsIndex />
        </q-toolbar-title>

        <UserInfo />
      </q-toolbar>

      <q-toolbar>

      </q-toolbar>
    </q-header>

    <q-drawer v-model="leftDrawerOpen" show-if-above bordered>
      <q-list>
        <q-item-label header>
          Essential Links
        </q-item-label>
      </q-list>
    </q-drawer>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
// 导入组件
import UserInfo from '../components/userInfo/userInfo.vue'
import BreadcrumbsIndex from '../components/breadcrumbs/breadcrumbsIndex.vue'

// #region 开关左侧抽屉
const leftDrawerOpen = ref(true)
const $q = useQuasar()
if ($q.platform.is.desktop) {
  leftDrawerOpen.value = true
}
function toggleLeftDrawer () {
  leftDrawerOpen.value = !leftDrawerOpen.value
}
// #endregion
</script>
