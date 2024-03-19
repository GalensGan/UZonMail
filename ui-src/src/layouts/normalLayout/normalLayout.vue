<template>
  <q-layout view="lHh lpR fFf">
    <q-header class="bg-white text-primary">
      <q-toolbar>
        <q-btn flat dense round icon="menu" aria-label="Menu" @click="toggleLeftDrawer" />
        <q-toolbar-title>
          <BreadcrumbsIndex />
        </q-toolbar-title>

        <UserInfo />
      </q-toolbar>

      <q-separator></q-separator>

      <div class="row justify-start q-ml-md">
        <TagsView />
      </div>
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

    <!--当菜单关闭且大屏幕时，才显示-->
    <q-footer v-if="showDock" class="bg-white">
      <SoftwareDock />
    </q-footer>
  </q-layout>
</template>

<script lang="ts" setup>
// 导入组件
import UserInfo from '../components/userInfo/userInfo.vue'
import BreadcrumbsIndex from '../components/breadcrumbs/breadcrumbsIndex.vue'
import TagsView from '../components/tags/tagsView.vue'

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

// #region 底部软件坞
const showDock = computed(() => {
  if ($q.screen.lt.md) return false
  if (leftDrawerOpen.value) return false
  return true
})
import SoftwareDock from '../components/softwareDock/softwareDock.vue'
// #endregion
</script>
