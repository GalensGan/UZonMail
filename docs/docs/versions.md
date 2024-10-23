---
title: 历史版本
---

## 0.10.0

> 2024-10-13

### 新增功能

1. 重构用户设置模块
2. 在发件时，可以增加取消订阅按钮。企业版本可用
3. 增加 docker 安装

### 下载地址

[uzonmail-desktop-win-x64-0.10.0.0.zip](https://obs.uamazing.cn:52443/public/files/soft/uzonmail-desktop-win-x64-0.10.0.0.zip)

[uzonmail-service-win-x64-0.10.0.0.zip](https://obs.uamazing.cn:52443/public/files/soft/uzonmail-service-win-x64-0.10.0.0.zip)

[uzonmail-service-linux-x64-0.10.0.0.zip](https://obs.uamazing.cn:52443/public/files/soft/uzonmail-service-linux-x64-0.10.0.0.zip)

[docker](https://hub.docker.com/r/gmxgalens/uzon-mail/tags)

## 0.9.5.1

> 2024-09-14

### bug 修复与优化

1. 启用跟踪后，提示 sequence contains no elements 问题

### 下载地址

[uzonmail-desktop-win-x64-0.9.5.1.zip](https://cloud.uamazing.cn:52443/#s/-zaqbDqg)

[uzonmail-service-linux-x64-0.9.5.1.zip](https://cloud.uamazing.cn:52443/#s/-zarK9AA)

## 0.9.5

> 2024-09-11

### 新增功能

1. 新增邮件阅读跟踪。在基础设置中启用邮件跟踪后，发件时会自动进行邮件跟踪

### bug 修复与优化

1. 修复定时任务启动后，无法找到发件组从而发件失败的 bug
2. 开始发件时，在数据处理时间内，增加等待进度条
3. 优化历史发件的状态显示
4. 优化发件箱错误后，未发邮件未能重置状态的 bug
5. 修复程序崩溃后，重新发送可能导致成功项重新发送的 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.5.0.zip](https://cloud.uamazing.cn:52443/#s/-yx5IyVA)

[uzonmail-service-linux-x64-0.9.5.0.zip](https://cloud.uamazing.cn:52443/#s/-yyMJlFw)

## 0.9.4

> 2024-09-04

### 新增功能

1. 文件管理右键支持分享功能：可以将文件通过链接的形式分享出去，可以通过这种方式将图片插入到模板中。分享功能需要服务器部署并且拥有域名。

### bug 修复

1. 修复基础设置修改失效的 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.4.0.zip](https://cloud.uamazing.cn:52443/#s/-xXJKHsw)

[uzonmail-service-linux-x64-0.9.4.0.zip](https://cloud.uamazing.cn:52443/#s/-xXJtdbg)

## 0.9.3.2

> 2024-09-03

### bug 修复

1. 修复发件箱报错后仍用于下次发件的 bug
2. 修复一级路由互相跳转时,layout 会刷新的 bug
3. 修复删除邮箱组报错
4. 修复当发件数据中的收件邮箱不存在于系统中时，收件数为空的 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.3.2.zip](https://cloud.uamazing.cn:52443/#s/-xKrNfbg)

[uzonmail-service-linux-x64-0.9.3.2.zip](https://cloud.uamazing.cn:52443/#s/-xKrufiA)

## 0.9.3.1

> 2024-09-02

### bug 修复

1. 修复数据库初始化报 Department 表错误

### 下载地址

[uzonmail-desktop-win-x64-0.9.3.1.zip](https://cloud.uamazing.cn:52443/#s/-w6vVVFQ)

[uzonmail-service-linux-x64-0.9.3.1.zip](https://cloud.uamazing.cn:52443/#s/-w6v6IGA)

## v0.9.3

`!!! 有严重 bug, 请勿使用`

> 2024-08-30

### 新增功能

1. 企业版本增加组织相关设置
2. 子账户将无法修改相关设置

### bug 修复

1. 修复证书过期的邮局无法发件的 bug
2. 修复无法设置子账户的 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.3.0.zip](https://cloud.uamazing.cn:52443/#s/-wYGIksg)

[uzonmail-service-linux-x64-0.9.3.0.zip](https://cloud.uamazing.cn:52443/#s/-wYGiIDQ)

## v0.9.2

> 2024-08-28

### 新增功能

1. 增加子账户功能(企业版)：主账户可以创建子账户，主账户具有管理子账户的设置、查看发件数据的功能
2. 增加管理员禁用其它账户功能

### bug 修复

1. 修复权限管理页面权限错误的 bug
2. 修复无法删除收件箱的 bug
3. 修复管理员无法重置用户密码的 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.2.0.zip](https://cloud.uamazing.cn:52443/#s/-v-KfpJw)

[uzonmail-service-linux-x64-0.9.2.0.zip](https://cloud.uamazing.cn:52443/#s/-v-K5ZMg)

## v0.9.1

> 2024-08-27

### 新增功能

1. 发件箱、收件箱支持批量导出
2. 发件明细支持按状态分类查看
3. 发件明细支持数据导出
4. 模板支持调整字体大小和设置颜色
5. 标签栏可拖动
6. 登陆页面增加版本显示，方便核对版本

### 下载地址

[uzonmail-desktop-win-x64-0.9.1.0.zip](https://cloud.uamazing.cn:52443/#s/-vw-iMhA)

[uzonmail-service-linux-x64-0.9.1.0.zip](https://cloud.uamazing.cn:52443/#s/-vw_NGIw)

## v0.9.0

### 新增功能

1. 增加专业版本和企业版本
2. 增加权限管理

### bug 修复

1. 修复无法修改发件箱 ssl 的 bug
2. 修复同时使用数据和发件箱时，报错 bug

### 下载地址

[uzonmail-desktop-win-x64-0.9.0.0.zip](https://cloud.uamazing.cn:52443/#s/-u66gFEw)

[uzonmail-service-linux-x64-0.9.0.0.zip](https://cloud.uamazing.cn:52443/#s/-vhdM6NA)

## v0.4.3

功能更新：

1. 支持非加密抄送
2. 主页显示到达率
3. 支持头像修改

## v0.4.2

功能更新：

1. 新增发送附件功能
   在新建附件中选择的附件是全局附件，每个收件箱都会收到，如果要针对个别收件箱发送不同的附件，可以在数据里面添加 attachments 字段不重写，多个文件用冒号分隔。

bug 修复：

1. 修复选择数据文件错误导致的发送失败问题

## v0.3.2

**新增功能：**

1. 增加默认变量 userName 和 inbox
2. 可通过数据中的 userName 自动关联收件人

## v0.3.1

**新增功能：**

1. 增加新建和编辑模板功能
2. 增加图文混发功能
3. 解除对模板数据必须输入的限制
4. 增加请求头伪装

## v0.2.1

数据重构版本
