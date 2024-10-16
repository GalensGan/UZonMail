import comp from "/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/versions.html.vue"
const data = JSON.parse("{\"path\":\"/versions.html\",\"title\":\"历史版本\",\"lang\":\"zh-CN\",\"frontmatter\":{\"title\":\"历史版本\"},\"headers\":[{\"level\":2,\"title\":\"v0.4.3\",\"slug\":\"v0-4-3\",\"link\":\"#v0-4-3\",\"children\":[]},{\"level\":2,\"title\":\"v0.4.2\",\"slug\":\"v0-4-2\",\"link\":\"#v0-4-2\",\"children\":[]},{\"level\":2,\"title\":\"v0.3.2\",\"slug\":\"v0-3-2\",\"link\":\"#v0-3-2\",\"children\":[]},{\"level\":2,\"title\":\"v0.3.1\",\"slug\":\"v0-3-1\",\"link\":\"#v0-3-1\",\"children\":[]},{\"level\":2,\"title\":\"v0.2.1\",\"slug\":\"v0-2-1\",\"link\":\"#v0-2-1\",\"children\":[]}],\"git\":{\"updatedTime\":null,\"contributors\":[]},\"filePathRelative\":\"versions.md\"}")
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
