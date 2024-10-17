import { defaultTheme } from '@vuepress/theme-default'
import { defineUserConfig } from 'vuepress/cli'
import { viteBundler } from '@vuepress/bundler-vite'

// 配置参考:https://v1.vuepress.vuejs.org/zh/theme/default-theme-config.html#%E9%A6%96%E9%A1%B5

export default defineUserConfig({
  lang: 'zh-CN',

  title: '宇正群邮',
  description: '一个开源强大的邮件批量发送系统',

  theme: defaultTheme({
    logo: '/images/logo.svg',
    search: true,
    searchMaxSuggestions: 10,
    navbar: [
      {
        text: '首页',
        link: '/'
      },
      {
        text: '文档',
        link: '/get-started',
        children: [
          {
            text: '开始使用',
            link: '/get-started'
          },
          {
            text: '视频介绍',
            link: '/video-introduction'
          },
          {
            text: '致谢名单',
            link: '/thanks-list'
          }
        ]
      },
      {
        text: '下载',
        link: '/versions'
      },
      {
        text: '联系我们',
        link: 'contact-us.md'
      },
      {
        text: '赞助支持',
        link: '/sponsor'
      },
      {
        text: 'GitHub',
        link: 'https://github.com/GalensGan/UZonMail'
      }
    ]
  }),

  bundler: viteBundler()
})
