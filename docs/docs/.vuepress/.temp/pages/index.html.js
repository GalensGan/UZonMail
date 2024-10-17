import comp from "/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/index.html.vue"
const data = JSON.parse("{\"path\":\"/\",\"title\":\"主页\",\"lang\":\"zh-CN\",\"frontmatter\":{\"home\":true,\"title\":\"主页\",\"heroImage\":\"/images/hero.svg\",\"actions\":[{\"text\":\"开始使用\",\"link\":\"/get-started\",\"type\":\"primary\"},{\"text\":\"软件下载\",\"link\":\"/versions\",\"type\":\"secondary\"}],\"features\":[{\"title\":\"多发件人同时发件\",\"details\":\"允许添加任意多个发件人进行同时发件, 突破单个发件数量限制, 提升发件效率。\"},{\"title\":\"自定义模板\",\"details\":\"模板采用 html 格式, 可视化编辑, 下限低，上限高。\"},{\"title\":\"无限变量\",\"details\":\"可以在模板中引入任意变量，实现发件内容因人而异。\"},{\"title\":\"多线程并发\",\"details\":\"采用多线程并发发送, 发件人越多，发件速度越快。\"},{\"title\":\"阅读跟踪\",\"details\":\"支持跟踪邮件的阅读状态\"},{\"title\":\"自带爬虫, 外贸神器\",\"details\":\"一次任务中允许添加任意多个收件箱。\"}],\"footer\":\"Apache-2.0 license | Copyright © 2021-persent UZonMail\"},\"headers\":[],\"git\":{\"updatedTime\":null,\"contributors\":[]},\"filePathRelative\":\"index.md\"}")
export { comp, data }

if (import.meta.webpackHot) {
  import.meta.webpackHot.accept()
  if (__VUE_HMR_RUNTIME__.updatePageData) {
    __VUE_HMR_RUNTIME__.updatePageData(data)
  }
}

if (import.meta.hot) {
  import.meta.hot.accept(({ data }) => {
    __VUE_HMR_RUNTIME__.updatePageData(data)
  })
}
