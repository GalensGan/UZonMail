<template>
  <q-input dense outlined class="full-width q-mb-md" standout v-model="password" :label="label"
    :type="isPwd ? 'password' : 'text'">
    <template v-slot:prepend>
      <q-icon name="lock" />
    </template>
    <template v-slot:append>
      <q-icon :name="isPwd ? 'visibility_off' : 'visibility'" class="cursor-pointer" @click="isPwd = !isPwd" />
    </template>
  </q-input>
</template>

<script lang="ts" setup>
defineProps({
  label: {
    type: String,
    default: '密码'
  }
})
const password = defineModel({
  type: String,
  required: true
})
const isPwd = ref(true)
</script>

<style lang="scss" scoped></style>
