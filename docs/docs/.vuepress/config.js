import { defaultTheme } from '@vuepress/theme-default'
import { defineUserConfig } from 'vuepress/cli'
import { viteBundler } from '@vuepress/bundler-vite'

export default defineUserConfig({
  lang: 'zh-CN',

  title: '宇正群邮',
  description: '一个开源强大的邮件批量发送系统',

  theme: defaultTheme({
    logo: '/images/logo.svg',
    navbar: [
      {
        text: '首页',
        link: '/'
      },
      {
        text: '开始使用',
        link: '/get-started'
      },
      {
        text: '历史版本',
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
