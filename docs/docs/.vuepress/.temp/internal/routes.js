export const redirects = JSON.parse("{}")

export const routes = Object.fromEntries([
  ["/contact-us.html", { loader: () => import(/* webpackChunkName: "contact-us.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/contact-us.html.js"), meta: {"title":"联系我们"} }],
  ["/get-started-vuepress.html", { loader: () => import(/* webpackChunkName: "get-started-vuepress.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/get-started-vuepress.html.js"), meta: {"title":"Get Started"} }],
  ["/get-started.html", { loader: () => import(/* webpackChunkName: "get-started.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/get-started.html.js"), meta: {"title":"开始使用"} }],
  ["/", { loader: () => import(/* webpackChunkName: "index.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/index.html.js"), meta: {"title":"主页"} }],
  ["/sponsor.html", { loader: () => import(/* webpackChunkName: "sponsor.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/sponsor.html.js"), meta: {"title":"支持作者"} }],
  ["/thanks-list.html", { loader: () => import(/* webpackChunkName: "thanks-list.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/thanks-list.html.js"), meta: {"title":""} }],
  ["/versions.html", { loader: () => import(/* webpackChunkName: "versions.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/versions.html.js"), meta: {"title":"历史版本"} }],
  ["/404.html", { loader: () => import(/* webpackChunkName: "404.html" */"/home/gmx/app/docker-dev-envs/uzonmail/src/docs/docs/.vuepress/.temp/pages/404.html.js"), meta: {"title":""} }],
]);

if (import.meta.webpackHot) {
  import.meta.webpackHot.accept()
  if (__VUE_HMR_RUNTIME__.updateRoutes) {
    __VUE_HMR_RUNTIME__.updateRoutes(routes)
  }
  if (__VUE_HMR_RUNTIME__.updateRedirects) {
    __VUE_HMR_RUNTIME__.updateRedirects(redirects)
  }
}

if (import.meta.hot) {
  import.meta.hot.accept(({ routes, redirects }) => {
    __VUE_HMR_RUNTIME__.updateRoutes(routes)
    __VUE_HMR_RUNTIME__.updateRedirects(redirects)
  })
}
