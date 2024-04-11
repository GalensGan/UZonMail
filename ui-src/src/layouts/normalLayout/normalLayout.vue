<template>
  <q-layout class="text-accent" view="lHh lpR fFf" dark>
    <q-header class="bg-grey-12 text-primary q-pa-md">
      <q-card class="column bg-white">
        <q-toolbar class="row items-center full-width">
          <MenuOpenButton v-model="drawer" />
          <q-toolbar-title>
            <BreadcrumbsIndex />
          </q-toolbar-title>
          <UserInfo />
        </q-toolbar>
        <q-separator></q-separator>
        <TagsView />
      </q-card>
    </q-header>

    <q-drawer v-model="drawer" show-if-above :mini="miniState" :width="300">
      <MenuTree />
    </q-drawer>

    <q-page-container class="page-container">
      <q-page class="q-px-md q-pb-md full-height full-with">
        <transition appear enter-active-class="animated fadeInDown">
          <router-view v-slot="{ Component }">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </router-view>
        </transition>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
// 导入组件
import MenuOpenButton from '../components/menuOpen/menuOpenButton.vue'
import UserInfo from '../components/userInfo/userInfo.vue'
import BreadcrumbsIndex from '../components/breadcrumbs/breadcrumbsIndex.vue'
import TagsView from '../components/tags/tagsView.vue'
// import LeftSidebarIndex from '../components/leftSidebar/leftSidebarIndex.vue'
import MenuTree from '../components/leftSidebar/menuTree.vue'

const drawer = ref(false)
const miniState = ref(false)
// const collapse = ref(false)

// #region 开关左侧抽屉
const $q = useQuasar()
if ($q.platform.is.desktop && $q.screen.gt.md) {
  drawer.value = true
}

// 缓存
// const cachedViews = computed(() => {
//   return []
// })

// const route = useRoute()
// const key = computed(() => {
//   return route.fullPath // this.$route.path
// })
</script>

<style lang="scss" scoped>
.menu-button {
  transform: scale(0.6);
}

.page-container {
  overflow: hidden;
  position: absolute;
  top: 0px;
  left: 0px;
  bottom: 0px;
  right: 0px;
  background-color: $grey-12;
}
</style>
