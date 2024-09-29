<template>
  <q-layout class="text-accent" view="lHh lpR fFf" dark>
    <q-page-container class="page-container">
      <q-page class="q-px-md q-pb-md full-height full-with">
        <q-scroll-area class="page__scroll-area full-height full-with" :thumb-style="thumbStyle"
          :content-style="contentStyle" :content-active-style="contentActiveStyle">
          <router-view v-slot="{ Component }">
            <transition appear enter-active-class="animated fadeInDown">
              <component :is="Component" :key="getRouteId($route.fullPath, $route.query)" />
            </transition>
          </router-view>
        </q-scroll-area>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script lang="ts" setup>
import { useScrollAreaStyle } from 'src/compositions/scrollUtils'
const { contentStyle, contentActiveStyle, thumbStyle } = useScrollAreaStyle()

import { getRouteId } from '../components/tags/routeHistories'
</script>

<style lang="scss" scoped></style>
