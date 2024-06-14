# README

## 🥝简介

![uzon-mail-login-2](https://obs.uamazing.cn:52443/public/files/images/uzon-mail-login-2.png)

本软件名为 “宇正群邮 (UZonMail)"，是一款开源免费的邮件群发软件。它具有非常强大的邮件正文定制能力，可快速实现 "千人千面" 式地批量群发，使每一封邮件都让人觉得很温暖。

原生多线程处理能力，极尽压榨每一核 CPU 性能，让发件体验纵享丝滑，盏茶间，群发任务灰飞烟灭，独怅惘，无件可发何时休。

常见的应用场景有：

1. 财务批量向员工发送每个人对应的工资条
2. 外贸批量群发营销邮件

<!--more-->

## 🍎特点

![image-20240614121857800](https://obs.uamazing.cn:52443/public/files/images/image-20240614121857800.png)

1. 支持多个发件人同时发件

   可以添加多个**不同的**发件人，同时发件，提高发件效率。

2. 支持多个收件人批量收件

   可以添加多个收件人，实现批量发送

3. 支持邮件内容模板自定义

   模板完全可自定义，可根据需要定义自己所需的模板，并保存到模板库，实现模板的复用。

   模板板采用 html 格式定义，程序也提供可视化界面进行编辑，对于新手和大神都很友好。

4. 支持无限变量，邮件封封不同

   可以在模板中引入变量，在发送的过程中，会自动将变量值替换成其真实的值进行发送，可以实现同一套模板，不同的收件人，接收的具体内容不同。

5. 多线程并发发送，日发可达 10 万+

   每个发件人采用单独的线程进行发送，当一个发件箱出问题之后并不会使发件停止，会由其它发件的所在线程继续发件。

   若有足够多的发件箱，日发 10 万不是问题。

6. 失败自动重发

   当有多个发件箱时，若 A 发件箱发件失败后，会转由 B 发件箱继续进行发送。

   如果仅有一个发件箱，当发件失败后，会在其它邮件发送完成后，再次发送，可重发次数最大为5次。

7. 支持失败手动重发

   所有的发送过程都有记录，对于未发送成功的邮件，可以在发送任务完成后，手动进行重发。

8. 支持发件箱每日发件总量限制

9. 支持抄送、密送

   支持多封邮件抄送到特定邮箱或者不同邮件抄送到不同的邮箱。

## 🍇环境要求

1. windows 7 及以上
2. .NET Framework 4.6.2 及以上，下载地址：[dotnet-framework](https://dotnet.microsoft.com/download/dotnet-framework)
3. webview2 环境，下载地址：[microsoft-edge/webview2/](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/)
4. ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)

## 🌱安装方式

### 安装包

[UZonMail-gitee](https://gitee.com/galensgan/UZonMail/releases/latest)

或者加 QQ 群 877458612 在群文件下载 (更新速度更快)

### 手动编译

1. 克隆仓库，切换到 `master` 分支；

2. 使用命令行进入到项目根目录，执行 `./build.ps1` 开始编译，编译结果在 `build` 目录中。

   编译成功截图：

   ![image-20240611220124980](https://obs.uamazing.cn:52443/public/files/images/image-20240611220124980.png)

> 手动编译时，会自动检测环境，若没有相关环境，请根据提示进行安装。

## 🍇更多帮助

详细文档已迁移至：[https://galens.uamazing.cn/posts/2020/2QMK677.html](https://galens.uamazing.cn/posts/2020/2QMK677.html)

