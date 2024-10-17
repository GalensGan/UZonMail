import comp from "/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/sponsor.html.vue"
const data = JSON.parse("{\"path\":\"/sponsor.html\",\"title\":\"支持作者\",\"lang\":\"zh-CN\",\"frontmatter\":{\"title\":\"支持作者\",\"sidebar\":false},\"headers\":[],\"git\":{\"updatedTime\":1729078977000,\"contributors\":[{\"name\":\"galens\",\"email\":\"gmx_galens@163.com\",\"commits\":1}]},\"filePathRelative\":\"sponsor.md\"}")
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
