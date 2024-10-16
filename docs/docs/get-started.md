---
uuid: 7deea49a-fd1e-11eb-ad13-597bfc858c85
title: 开始使用
tags: 
  - UzonMail
updated: 2022-01-19
abbrlink: 2QMK677
---

## 🥝简介

![uzon-mail-login-2](https://obs.uamazing.cn:52443/public/files/images/uzon-mail-login-2.png)

本软件名为 “宇正群邮 (UZonMail)"，是一款开源免费的邮件群发软件。它具有非常强大的邮件正文定制能力，可快速实现 "千人千面" 式地批量群发，使每一封邮件都让人觉得很温暖。

原生多线程处理能力，极尽压榨每一核 CPU 性能，让发件体验纵享丝滑，盏茶间，群发任务灰飞烟灭，独怅惘，无件可发何时休。

常见的应用场景有：

1. 财务批量向员工发送每个人对应的工资条
2. 外贸批量群发营销邮件

> 开源地址：[UZonMail](https://github.com/GalensGan/SendMultipleEmails)

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

1. Windows 7 及以上
2. .NET Framework 4.6.2 及以上，下载地址：[dotnet-framework](https://dotnet.microsoft.com/download/dotnet-framework)
3. Webview2 环境，下载地址：[microsoft-edge/webview2/](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/)
4. ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)

## 🌱Windows 安装使用

### 环境安装

**Win7**:

1. .NET Framework 4.6.2 及以上（这个一般都有，可以不用安装），下载地址：[dotnet-framework](https://dotnet.microsoft.com/download/dotnet-framework)
2. Webview2 环境，下载地址：[microsoft-edge/webview2/-腾讯微云](https://share.weiyun.com/RAh0rLTA) 
3. ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)

> 温馨提示：
>
> win7 与 win10 使用的 webview2 版本不一致

**Win10 及以上**：

1. .NET Framework 4.6.2 及以上，下载地址：[dotnet-framework](https://dotnet.microsoft.com/download/dotnet-framework)
2. Webview2 环境，下载地址：[microsoft-edge/webview2/](https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/)
3. ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)

**网页版**：

1. ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)

### 直接下载安装包

[UZonMail-gitee](https://gitee.com/galensgan/UZonMail/releases/latest)

或者加 QQ 群 877458612 在群文件下载 (更新速度更快)

### 自己手动编译

1. 克隆仓库，切换到 `master` 分支；

2. 使用命令行进入到项目根目录，执行 `./build.ps1` 开始编译，编译结果在 `build` 目录中。

   编译成功截图：

   ![image-20240616124656131](https://obs.uamazing.cn:52443/public/files/images/image-20240616124656131.png)

> 手动编译时，会自动检测环境，若没有相关环境，请根据提示进行安装。

### 使用

本节只讲述通过安装包的使用方式。

1. 下载安装包
2. 安装对应方式所需要的环境

直接使用：

解压后，直接打开文件 `UzonMailDesktop.exe` 即可开始使用

网页端使用：

1. 首先启动服务：打开 `UzonMailDesktop.exe` 或 `service/UZonMailService.exe` 文件
2. 在浏览器中输入地址：`http://localhost:22345` 打开网页

## 🍒发件步骤

1. 添加发件箱（已添加请忽略）
2. 添加收件箱（已添加请忽略）
3. 导入所需模板（已添加请忽略）
4. 打开【新建发件】，输入主题 --> 选择收件人 --> 选择模板 --> 选择数据 -->预览确认发件数量和模板正确性 --> 退出预览 -->点击【发送】
5. 如果提示发送失败，转到【发件历史】，进行重发

### 邮件经常进发件箱怎么办

可以使用 [Newsletters spam test by mail-tester.com](https://www.mail-tester.com/) 检查待发邮件的内容。

![image-20220428140629151](https://cdn.jsdelivr.net/gh/GalensGan/objects-storage/images/image-20220428140629151.png)

## 🥑功能介绍

### 功能概览

![image-20240614121857800](https://obs.uamazing.cn:52443/public/files/images/image-20240614121857800.png)

软件主要由以下功能模块组成：

- 首页

  数据统计展示

- 邮箱管理

  管理发件箱、收件箱

- 模板管理

  管理正文模板

- 发件管理

  新建发件和历史发件查询

- 系统设置

  - 用户管理

    增加其它使用用户

  - 基础设置

    发件相关的全局设置

  - 代理管理

    配置发件代理

- 支持作者

  赞助页面

- 使用说明

  帮助页

下面开始按模块进行详细介绍。

### 登陆界面

![uzon-mail-login-2](https://obs.uamazing.cn:52443/public/files/images/uzon-mail-login-2.png)

默认用户名和密码为：admin/admin1234

### 首页

![image-20240614231957130](https://obs.uamazing.cn:52443/public/files/images/image-20240614231957130.png)

首页主要展示的内容有：

- 发件箱数量直方图
- 收件箱数量直方图
- 每月发件量折线图

### 系统设置

#### 用户管理

![image-20240612122713293](https://obs.uamazing.cn:52443/public/files/images/image-20240612122713293.png)

系统默认的用户名为 admin，默认密码为 admin1234，这是一个管理员账号，该账号具有【管理用户】的权限。

【用户管理】模块用于增加不同的用户。桌面版本的多用户功能仅限本机使用，若要多人同时使用，则需要使用服务器版本。

服务器版本可联系作者获取。

**新增用户：**

![image-20240612123329057](https://obs.uamazing.cn:52443/public/files/images/image-20240612123329057.png)

单击左上角的新增，即可新增用户。

用户新增完成后，可对用户进行操作，比如重置密码，删除等

重置后的密码为：`uzonmail123`，在重置时会有提示。

![image-20240612123429178](https://obs.uamazing.cn:52443/public/files/images/image-20240612123429178.png)

**修改密码和头像：**

可以通过右上角的个人信息界面对头像和密码进行修改。

![image-20240612125131168](https://obs.uamazing.cn:52443/public/files/images/image-20240612125131168.png)

#### 基础设置

基础设置中，主要设置全局的发件间隔，最大发件量等。

![image-20240612125859579](https://obs.uamazing.cn:52443/public/files/images/image-20240612125859579.png)

- 单个邮箱每日最大发件量

  控制单个邮箱每日发件的总数，避免因发件数量超过每个邮件服务提供商规定的每日发件量，从而导致发件失败。

  为 0 时表示不限制

- 单个发件箱最小(最大)发件间隔

  单位：秒

  为了避免因频繁发送邮件而导致被服务器认为是垃圾邮箱，所以，发送两封邮件之间需要有一定的时间间隔，为了使得发送时间间隔具有不规律性，用间隔范围来进行控制：

  实际发件间隔值 = 最小值 + （0，1）之间的随机数*（最大值-最小值）

  最大值小于等于最小值时，表示不限制

- 合并发件最大数量

  当同时向多个收件箱发送相同内容时，可以将收件箱合并成一封邮件发送，这个参数即控制合并的最大数量。

  每个邮件服务商允许的最大数量不一样，最大数包含抄送和密送的数量

  为 0 时，表示不合并

#### 代理管理

![image-20240612131312091](https://obs.uamazing.cn:52443/public/files/images/image-20240612131312091.png)

代理管理模块主要针对使用国外邮箱的情况，允许针对某一类或者某个发件箱指定代理。

该功能一般用于服务器部署的情况，本机使用时，可以打开全局代理。

**新增代理**：

![image-20240612130111084](https://obs.uamazing.cn:52443/public/files/images/image-20240612130111084.png)

新增代理参数说明：

- 名称

  必填项。在发件箱界面，可通过名称选择指定代理

- 代理地址

  代理的地址、用户名和密码。格式为：`协议:\\username:password@host`，示例：

  1. 完整格式：`http:\\admin:admin1234@127.0.0.1:7890`
  2. 无密码格式: `http:\\127.0.0.1:7890`
  3. 其它协议：`socket5:\\127.0.0.1:7890`

  目前支持的协议有：`http、https、socks4、socks4a、socket5`

- 匹配规则

  若发件箱没有指定代理，则会从代理管理的列表中根据规则自动匹配，若匹配到，则使用。

  该规则的语法为正则表达式

  `.*` 表示所有的邮件都匹配

- 优先级

  规则匹配的优先级

- 是否共享

  共享后，系统内所有人都可使用这个代理

> 代理安全提示
>
> 代理是明文存储在服务器中的，因此管理员可以查看代理的信息，可能会造成代理泄露风险，请谨慎添加个人代理

### 邮箱管理

#### 发件箱

【发件箱】模块用于管理发件人信息。下面列出在使用中需要注意的功能进行说明：

**组管理：**

增加发件箱时，必须先建立发件组。

![image-20240612131552355](https://obs.uamazing.cn:52443/public/files/images/image-20240612131552355.png)

在 "发件箱" 上右键，可新增发件组。

![image-20240612131711338](https://obs.uamazing.cn:52443/public/files/images/image-20240612131711338.png)

新增时，"序号" 表示发件组的排序号，仅用于排序。

当新建组完成后，可以在组名上右键，对组进行管理

![image-20240612131916319](https://obs.uamazing.cn:52443/public/files/images/image-20240612131916319.png)

**新增发件箱**：

![image-20240612132056895](https://obs.uamazing.cn:52443/public/files/images/image-20240612132056895.png)

本软件采用的是 SMTP 协议发件，因此发件时，需要将自己的邮箱开通 SMTP 服务，可自行查阅资料。以下对一些重要参数说明：

- 发件人名称

  若有，当发给对方后，不显示邮箱，而是显示名称

- smtp 密码

  发件采用的 SMTP 服务器，所以，它的密码并不是邮箱的密码，而是登陆邮箱后，自己申请的 SMTP 密码。

  比如，163邮箱 SMTP 密码获取方式如下：https://www.yisu.com/zixun/97973.html

- smtp 地址

  smtp 的地址，每种类型的地址不一样，可百度查找

- smtp 端口

  该端口与是否【启用 SSL】有关，默认为 25，若启动 SSL，一般为 465，需要自动查找确认。

- 启用 SSL

  是在发件时，采用 SSL 加密，打开这个可以提升发件的安全性。

- 代理

  若有需要，可以为其指定代理，可用代理候选项在【代理管理】中定义。

> 密码安全提示
>
> 服务器没有直接存储 smtp 密码，而是存储了通过密钥加密后的密文，密钥由前端生成，当有需要时，由前端传递给后端解密。
>
> 因此，即使数据库泄露了，也不会造成 smtp 的密码被盗的问题

**从EXCEL导入：**

通过【导入】功能可批量从 Excel 中导入发件箱。

可以通过【模板】按钮下载导入模板。

在使用批量添加发件人功能时，Excel 表中第一行为表头，必须包含 `smtp邮箱`、`smtp密码`、`smtp地址`、`smtp端口`  列。

![image-20240614123302248](https://obs.uamazing.cn:52443/public/files/images/image-20240614123302248.png)

#### 收件箱

![image-20240614124814856](https://obs.uamazing.cn:52443/public/files/images/image-20240614124814856.png)

该模块主要用于对收件箱的分组和管理，使用方式、注意要点与发件人一致。

收件箱只需要姓名和邮箱即可，姓名是可选的。

### 模板管理

![image-20240614125056768](https://obs.uamazing.cn:52443/public/files/images/image-20240614125056768.png)

在【正文模板】用于管理用户下的所有模板，它是 html 格式。

#### 新增模板

使用两种方式进行添加：

- 导入 HTML
  
  先在外面用 html 定义好模板，然后通过上述中的【导入模板】功能将定义的模板导入到系统。对于自定义的 html 模板，要求其中的 css 必须为行内 css。可以通过 http://automattic.github.io/juice/ 自动将 html 文件中 css 转换成行内的 css。

- 直接编辑
  
  通过单击【新增】按钮新增模板。
  
  ![image-20240614125417120](https://obs.uamazing.cn:52443/public/files/images/image-20240614125417120.png)

#### 模板编辑

通过单击模板名称或者在模板上右键，然后选择【编译】跳转到模板修改界面。

![image-20240614125609679](https://obs.uamazing.cn:52443/public/files/images/image-20240614125609679.png)

#### 模板变量

在模板的编写过程中，可以使用双花括号（`{{变量名}}`）来标记为变量，在发件的过程中，程序会在数据中查找该变量，如果查找到，就会使用实际的数据将变量替换掉。

变量定义的格式是为：&#123;&#123;变量名&#125;&#125;。

> 在发件中，模板也可以因发件人而异，需要在数据中增加 templateId 列来覆盖通用的模板。具体参考发件篇。

### 发件管理

#### 新建发件

![image-20240614125845479](https://obs.uamazing.cn:52443/public/files/images/image-20240614125845479.png)

新建发件用于添加发件任务，通过不同的参数组合，它可以实现以下功能：

1. 一对一发件

   一个发件箱，一个收件箱

2. 一对多发件

   一个发件箱，多个收件箱

3. 多对多发件

   多个发件箱，多个收件箱

4. 主题变化、正文变化发件

   同时支持主题和正文根据收件人不同而变化

##### 主题

发件的主题是必须的，主有两个作用：一是为邮件的主题，二是同一次发件将会归到一个发件历史记录中，该主题为历史记录组的名称。

多个主题使用英文分号（`;`）或者换行进行分隔。

![image-20240614130827129](https://obs.uamazing.cn:52443/public/files/images/image-20240614130827129.png)

若有多个主题，系统在发件时，会随机使用一个主题（若在数据中指定了主题，则会固定使用数据的主题）。

主题也支持变量声明，比如：&#123;&#123;日期&#125;&#125;-工资明细，`日期` 即为定义的变量，在发送邮件时，它将被替换成 Excel 表中的实际数据。

##### 模板

模板相当于是正文的一个草稿，它可以让你快速发送正文，而不需要每次都在正文处输入。

![image-20240614131034812](https://obs.uamazing.cn:52443/public/files/images/image-20240614131034812.png)

可以选择多个模板。若有多个模板时，系统将随机选择一个模板来发件（若在数据中指定了模板，则会固定使用数据中的模板）

##### 正文

![image-20240614131326894](https://obs.uamazing.cn:52443/public/files/images/image-20240614131326894.png)

![image-20240614225842011](https://obs.uamazing.cn:52443/public/files/images/image-20240614225842011.png)

软件支持用户手动输入正文。

若用户指定了正文，则不会使用模板作为邮件正文。

正文的格式与模板一样，同样支持变量。

##### 发件人

![image-20240614131606189](https://obs.uamazing.cn:52443/public/files/images/image-20240614131606189.png)

单击发件人右侧的 + 号，选择发件人。

发件人允许有多个，若有多个发件人，发件时，将会把邮件随机给其中一个发件人发件。

在一次任务中，一封邮件只会被其中一个发件箱成功发送，不会多次发送。

##### 收件人

![image-20240614131851665](https://obs.uamazing.cn:52443/public/files/images/image-20240614131851665.png)

单击收件人右侧的 + 号，选择收件人。

单次发件任务中，允许添加多个收件人，每个收件都会收到一款邮件。

##### 抄送人

若选择抄送人，每一封邮件都会抄送到每一个抄送人处。

##### 附件

![image-20240614214517919](https://obs.uamazing.cn:52443/public/files/images/image-20240614214517919.png)

若邮件中需要添加附件，可以在此添加附件。

允许添加多个附件，但是请注意，每封邮件都会携带相同的附件。

##### 数据

支持数据发件功能是该软件的灵魂。通过导入数据，可以实现一次发件中，为不同的收件箱发送不同的内容。

当将鼠标聚焦在数据栏，右侧会出现下载模板的图标，单击该图标即可下载模板。

![image-20240614215520531](https://obs.uamazing.cn:52443/public/files/images/image-20240614215520531.png)

数据格式大致如下：

![image-20240614220243117](https://obs.uamazing.cn:52443/public/files/images/image-20240614220243117.png)

**数据的效果**：

模板内容:

![image-20240614222544153](https://obs.uamazing.cn:52443/public/files/images/image-20240614222544153.png)

模板赋予数据后的正文预览：

![image-20240614222617857](https://obs.uamazing.cn:52443/public/files/images/image-20240614222617857.png)

**数据的作用**：

1. 为模板提供变量
2. 快速实现精准的批量发送

**数据中系统保留变量**：

| 变量名       | 必须 | 描述                                                         |
| ------------ | ---- | ------------------------------------------------------------ |
| inbox        | 是   | 指定收件邮箱。该字段必须存在，程序依靠该变量进行发件匹配。若为空，则该行数据无效。 |
| inboxName    | 否   | 设置收件人名称。                                             |
| subject      | 否   | 指定主题。若指定，则会忽略界面中输入的主题。                 |
| outbox       | 否   | 指定发件箱。若不指定，则使用用户在界面中选择的发件箱。<br />该发件箱必须是在【邮箱管理/发件箱】中添加的邮箱地址。其它邮箱则视为无效。 |
| outboxName   | 否   | 设置发件箱名称。若不指定，则使用发件箱管理中的名称。         |
| cc           | 否   | 指定抄送人。多个抄送人使用逗号分隔。                         |
| templateId   | 否   | 指定邮件的模板 Id。该模板 Id 可在模板管理中查看，是一个数字。<br />若不指定，则从用户选择的模板中随机取一个使用。 |
| templateName | 否   | 指定邮件的模板名称。该名称可在模板管理中查看。templateName 的优先级低于 templateId。当两者同时指定时，以 templateId 为主。<br />若不指定，则从用户选择的模板中随机取一个使用。 |
| body         | 否   | 指定邮件的正文内容。该优先级大于 templateId 和 templateName。 |

**数据的优先级**：

- 主题

  【Excel数据/subject】  > 【界面/主题】

- 正文

  【Excel数据/body】 > 【Excel数据/templateId】 > 【Excel数据/templateName】 > 【界面/正文】 >【 界面/模板 】

- 发件人

  【Excel数据/outbox 】>【 界面/发件人】

- 收件人

  【Excel数据/inbox 】> 【界面/收件人】

- 抄送人

  【Excel数据/cc】 > 【界面/抄送人】

- 附件

  附件目前无法通过数据指定

#### 发件历史

发件历史显示历次所发的所有邮件记录，一次发送任务为一条历史。

单击 ID 或者右键【详细】可查看具体发件项。

![image-20240614225346191](https://obs.uamazing.cn:52443/public/files/images/image-20240614225346191.png)

发件明细：

![image-20240614231145776](https://obs.uamazing.cn:52443/public/files/images/image-20240614231145776.png)

## 🪺技术栈

本系统采用前后端分离的模式进行开发。

**前端：**

typescript+vue3+quasar

**后端：**

C# + WPF +ASP.NET Core+webview2

使用 ASP.NET Core 实现服务端，桌面端使用 WPF 开发

## 🍄服务器部署

### 简易版

1. 安装 ASP.NET Core 环境，下载地址：[runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer)
2. 启动 `service/UZonMailService.exe`

至此，即可通过 `http://you-ip:22345` 进行访问了

### 前后端分离部署

到这一步，我相信您已经具备专业的知识，可以自己处理安装，此处只简述下前后端各部分内容：

1. 后端位于 `service` 目录
2. 前端位于 `service/wwwroot` 目录

您可以配置 nginx 进行反向代理，配置负载均衡

### 前后端配置说明

前端的配置位于 `service/wwwroot/app.config.ts` 文件中，若是对外提供服务，则需要修改后端接口地址，即 `baseUrl`：

``` typescript
export default {
  // 默认配置
  default: {
    baseUrl: 'http://localhost:22345',
    api: '/api/v1',
    signalRHub: '/hubs/uzonMailHub'
  },

  // 生产配置
  prod: {},

  // 开发配置
  dev: {}
} as Record<'default' | 'prod' | 'dev', any>
```

后端的配置位于 `service/wwwroot/appsettings.json`，可以在这里配置：

- 日志
- 授权密钥
- 系统参数
- 接口 Http 参数
- Websocket 参数
- 数据库参数
- 初始用户参数
- 跨域
- 文件存储
- 定时任务

请查看配置文件进行理解。

## 🌵反馈与建议

如果你在使用中发现了 bug, 或者对该软件有任何建议，都欢迎联系我，让我们将这款软件一起变得更优秀吧！

bug 反馈优先选择[Github Issues](https://github.com/GalensGan/SendMultipleEmails/issues)，这样我能第一时间知道。

如果有紧急问题，请通过邮件联系。

QQ 和 QQ 群会不定时查看，所以不会很及时。

## 🌶️联系方式

QQ群：877458612

邮箱：260827400@qq.com

GitHub：[GalensGan/UZonMail (github.com)](https://github.com/GalensGan/UZonMail)

个人主页：[https://galens.uamazing.cn](https://galens.uamazing.cn)

## 🍉致谢名单

感谢老铁们对该软件的肯定，感谢大家的支持与鼓励！

感谢 QQ 用户 `来了来了`、`Me` 协助管理 QQ 群。

感谢以下用户的赞助（按先后时间排序）：

磊、鹏