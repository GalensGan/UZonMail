import comp from "/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/contact-us.html.vue"
const data = JSON.parse("{\"path\":\"/contact-us.html\",\"title\":\"联系我们\",\"lang\":\"zh-CN\",\"frontmatter\":{\"title\":\"联系我们\"},\"headers\":[{\"level\":2,\"title\":\"🌵反馈与建议\",\"slug\":\"🌵反馈与建议\",\"link\":\"#🌵反馈与建议\",\"children\":[]},{\"level\":2,\"title\":\"🌶️技术支持\",\"slug\":\"🌶️技术支持\",\"link\":\"#🌶️技术支持\",\"children\":[]}],\"git\":{\"updatedTime\":null,\"contributors\":[]},\"filePathRelative\":\"contact-us.md\"}")
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
