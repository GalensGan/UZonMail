export const themeData = JSON.parse("{\"logo\":\"/images/logo.svg\",\"search\":true,\"searchMaxSuggestions\":10,\"navbar\":[{\"text\":\"首页\",\"link\":\"/\"},{\"text\":\"文档\",\"link\":\"/get-started\",\"children\":[{\"text\":\"开始使用\",\"link\":\"/get-started\"},{\"text\":\"视频介绍\",\"link\":\"/video-introduction\"},{\"text\":\"致谢名单\",\"link\":\"/thanks-list\"}]},{\"text\":\"下载\",\"link\":\"/versions\"},{\"text\":\"联系我们\",\"link\":\"contact-us.md\"},{\"text\":\"赞助支持\",\"link\":\"/sponsor\"},{\"text\":\"GitHub\",\"link\":\"https://github.com/GalensGan/UZonMail\"}],\"locales\":{\"/\":{\"selectLanguageName\":\"English\"}},\"colorMode\":\"auto\",\"colorModeSwitch\":true,\"repo\":null,\"selectLanguageText\":\"Languages\",\"selectLanguageAriaLabel\":\"Select language\",\"sidebar\":\"heading\",\"sidebarDepth\":2,\"editLink\":true,\"editLinkText\":\"Edit this page\",\"lastUpdated\":true,\"lastUpdatedText\":\"Last Updated\",\"contributors\":true,\"contributorsText\":\"Contributors\",\"notFound\":[\"There's nothing here.\",\"How did we get here?\",\"That's a Four-Oh-Four.\",\"Looks like we've got some broken links.\"],\"backToHome\":\"Take me home\",\"openInNewWindow\":\"open in new window\",\"toggleColorMode\":\"toggle color mode\",\"toggleSidebar\":\"toggle sidebar\"}")

if (import.meta.webpackHot) {
  import.meta.webpackHot.accept()
  if (__VUE_HMR_RUNTIME__.updateThemeData) {
    __VUE_HMR_RUNTIME__.updateThemeData(themeData)
  }
}

if (import.meta.hot) {
  import.meta.hot.accept(({ themeData }) => {
    __VUE_HMR_RUNTIME__.updateThemeData(themeData)
  })
}
