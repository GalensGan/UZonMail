<template>
  <div class="hover_overlay_container">
    <slot></slot>
    <slot name="hover"></slot>
  </div>
</template>

<script lang="ts" setup>

</script>

<style lang="scss" scoped>
.hover_overlay_container {
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  position: absolute;
  top: 0;
  left: 0;
  z-index: 100;
  display: none;
  justify-content: center;
  align-items: center;
  color: white;
  font-size: 20px;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.5s;
}
</style>
