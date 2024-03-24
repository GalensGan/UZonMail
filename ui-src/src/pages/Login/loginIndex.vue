<template>
  <div class="column justify-center login-container">

    <div class="row justify-center items-center q-mb-xl">
      <q-icon name="img:icons/undraw_mailbox_re_dvds.svg" size="200px" class="q-ma-lg"></q-icon>

      <q-card class="q-ma-md q-pa-lg column justify-center items-center" style="min-width: 400px;">
        <div class="self-center q-mb-lg text-h5 text-accent">Welcome UZonMail</div>

        <q-input class="full-width q-mb-md" standout v-model="userId" label="用户名">
          <template v-slot:prepend>
            <q-icon name="place" />
          </template>
        </q-input>

        <q-input class="full-width" standout v-model="userId" label="用户名">
          <template v-slot:prepend>
            <q-icon name="place" />
          </template>
        </q-input>
      </q-card>
    </div>
  </div>
</template>

<script lang="ts" setup>
// 登陆界面
const userId = ref('')
// const password = ref('')
</script>

<style lang="scss" scoped>
.login-container {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
}
</style>
