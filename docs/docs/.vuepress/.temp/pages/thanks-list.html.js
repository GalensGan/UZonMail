import comp from "/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/thanks-list.html.vue"
const data = JSON.parse("{\"path\":\"/thanks-list.html\",\"title\":\"\",\"lang\":\"zh-CN\",\"frontmatter\":{},\"headers\":[{\"level\":2,\"title\":\"🍉致谢名单\",\"slug\":\"🍉致谢名单\",\"link\":\"#🍉致谢名单\",\"children\":[]}],\"git\":{\"updatedTime\":null,\"contributors\":[]},\"filePathRelative\":\"thanks-list.md\"}")
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
