<template>
  <div class="column justify-center login-container bg-white">
    <div class="row justify-center items-center q-mb-xl animated fadeIn slower">
      <q-icon :name="resolveSvgFullName('undraw_mention_re_k5xc')" size="220px"
        class="q-pa-lg animated fadeInUp"></q-icon>

      <div
        class="q-ma-md q-pa-lg column justify-center items-center border-radius-8 animated fadeInDown hover-card card-like"
        style="width: 400px;" @keyup.enter="onUserLogin">
        <div class="self-center q-mb-lg text-h5 text-secondary welcome-to-uzon-mail">Welcome to UZonMail</div>

        <q-input outlined class="full-width q-mb-md" standout v-model="userId" label="用户名">
          <template v-slot:prepend>
            <q-icon name="person" />
          </template>
        </q-input>

        <q-input outlined class="full-width q-mb-md" standout v-model="password" label="密码"
          :type="isPwd ? 'password' : 'text'">
          <template v-slot:prepend>
            <q-icon name="lock" />
          </template>
          <template v-slot:append>
            <q-icon :name="isPwd ? 'visibility_off' : 'visibility'" class="cursor-pointer" @click="isPwd = !isPwd" />
          </template>
        </q-input>

        <q-btn class="full-width border-radius-8" color="primary" label="登 陆" @click="onUserLogin" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { userLogin } from 'src/api/user'
import { useUserInfoStore } from 'src/stores/user'
import { notifyError } from 'src/utils/dialog'
import { resolveSvgFullName } from 'src/utils/svgHelper'
import { md5 } from 'src/utils/encrypt'

// 登陆界面
const userId = ref('')
const password = ref('')
const isPwd = ref(true)

const router = useRouter()
/**
 * 用户登陆
 */
async function onUserLogin () {
  // 验证数据
  if (!userId.value) {
    notifyError('请输入用户名')
    return
  }

  if (!password.value) {
    notifyError('请输入密码')
    return
  }

  // 登陆逻辑
  // 1- 请求登陆信息，返回用户信息、token、权限信息
  // 2- 保存信息、密码加密后保存，用于解析服务器的密码
  // 3- 跳转到主页或重定向的页面
  const { data: { userInfo, token, access } } = await userLogin(userId.value, password.value)
  const userInfoStore = useUserInfoStore()
  userInfoStore.setUserLoginInfo(userInfo, token, access)
  userInfoStore.setSecretKey(md5(password.value))

  console.log('登陆成功:', userInfo, token, access)
  // 跳转到主页
  router.push({ path: '/' })
}

</script>

<style lang="scss" scoped>
.login-container {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;

  .welcome-to-uzon-mail {
    background: -webkit-linear-gradient(315deg, #42d392 25%, #857bf0);
    background-clip: text;
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
  }
}
</style>
