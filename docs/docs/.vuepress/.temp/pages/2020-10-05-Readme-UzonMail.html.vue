<template><div><h2 id="🥝简介" tabindex="-1"><a class="header-anchor" href="#🥝简介"><span>🥝简介</span></a></h2>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/uzon-mail-login-2.png" alt="uzon-mail-login-2"></p>
<p>本软件名为 “宇正群邮 (UZonMail)&quot;，是一款开源免费的邮件群发软件。它具有非常强大的邮件正文定制能力，可快速实现 &quot;千人千面&quot; 式地批量群发，使每一封邮件都让人觉得很温暖。</p>
<p>原生多线程处理能力，极尽压榨每一核 CPU 性能，让发件体验纵享丝滑，盏茶间，群发任务灰飞烟灭，独怅惘，无件可发何时休。</p>
<p>常见的应用场景有：</p>
<ol>
<li>财务批量向员工发送每个人对应的工资条</li>
<li>外贸批量群发营销邮件</li>
</ol>
<blockquote>
<p>开源地址：<a href="https://github.com/GalensGan/SendMultipleEmails" target="_blank" rel="noopener noreferrer">UZonMail</a></p>
</blockquote>
<!--more-->
<h2 id="🍎特点" tabindex="-1"><a class="header-anchor" href="#🍎特点"><span>🍎特点</span></a></h2>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614121857800.png" alt="image-20240614121857800"></p>
<ol>
<li>
<p>支持多个发件人同时发件</p>
<p>可以添加多个<strong>不同的</strong>发件人，同时发件，提高发件效率。</p>
</li>
<li>
<p>支持多个收件人批量收件</p>
<p>可以添加多个收件人，实现批量发送</p>
</li>
<li>
<p>支持邮件内容模板自定义</p>
<p>模板完全可自定义，可根据需要定义自己所需的模板，并保存到模板库，实现模板的复用。</p>
<p>模板板采用 html 格式定义，程序也提供可视化界面进行编辑，对于新手和大神都很友好。</p>
</li>
<li>
<p>支持无限变量，邮件封封不同</p>
<p>可以在模板中引入变量，在发送的过程中，会自动将变量值替换成其真实的值进行发送，可以实现同一套模板，不同的收件人，接收的具体内容不同。</p>
</li>
<li>
<p>多线程并发发送，日发可达 10 万+</p>
<p>每个发件人采用单独的线程进行发送，当一个发件箱出问题之后并不会使发件停止，会由其它发件的所在线程继续发件。</p>
<p>若有足够多的发件箱，日发 10 万不是问题。</p>
</li>
<li>
<p>失败自动重发</p>
<p>当有多个发件箱时，若 A 发件箱发件失败后，会转由 B 发件箱继续进行发送。</p>
<p>如果仅有一个发件箱，当发件失败后，会在其它邮件发送完成后，再次发送，可重发次数最大为5次。</p>
</li>
<li>
<p>支持失败手动重发</p>
<p>所有的发送过程都有记录，对于未发送成功的邮件，可以在发送任务完成后，手动进行重发。</p>
</li>
<li>
<p>支持发件箱每日发件总量限制</p>
</li>
<li>
<p>支持抄送、密送</p>
<p>支持多封邮件抄送到特定邮箱或者不同邮件抄送到不同的邮箱。</p>
</li>
</ol>
<h2 id="🍇环境要求" tabindex="-1"><a class="header-anchor" href="#🍇环境要求"><span>🍇环境要求</span></a></h2>
<ol>
<li>Windows 7 及以上</li>
<li>.NET Framework 4.6.2 及以上，下载地址：<a href="https://dotnet.microsoft.com/download/dotnet-framework" target="_blank" rel="noopener noreferrer">dotnet-framework</a></li>
<li>Webview2 环境，下载地址：<a href="https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/" target="_blank" rel="noopener noreferrer">microsoft-edge/webview2/</a></li>
<li>ASP.NET Core 环境，下载地址：<a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer" target="_blank" rel="noopener noreferrer">runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer</a></li>
</ol>
<h2 id="🌱windows-安装使用" tabindex="-1"><a class="header-anchor" href="#🌱windows-安装使用"><span>🌱Windows 安装使用</span></a></h2>
<h3 id="环境安装" tabindex="-1"><a class="header-anchor" href="#环境安装"><span>环境安装</span></a></h3>
<p><strong>Win7</strong>:</p>
<ol>
<li>.NET Framework 4.6.2 及以上（这个一般都有，可以不用安装），下载地址：<a href="https://dotnet.microsoft.com/download/dotnet-framework" target="_blank" rel="noopener noreferrer">dotnet-framework</a></li>
<li>Webview2 环境，下载地址：<a href="https://share.weiyun.com/RAh0rLTA" target="_blank" rel="noopener noreferrer">microsoft-edge/webview2/-腾讯微云</a></li>
<li>ASP.NET Core 环境，下载地址：<a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer" target="_blank" rel="noopener noreferrer">runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer</a></li>
</ol>
<blockquote>
<p>温馨提示：</p>
<p>win7 与 win10 使用的 webview2 版本不一致</p>
</blockquote>
<p><strong>Win10 及以上</strong>：</p>
<ol>
<li>.NET Framework 4.6.2 及以上，下载地址：<a href="https://dotnet.microsoft.com/download/dotnet-framework" target="_blank" rel="noopener noreferrer">dotnet-framework</a></li>
<li>Webview2 环境，下载地址：<a href="https://developer.microsoft.com/zh-cn/microsoft-edge/webview2/" target="_blank" rel="noopener noreferrer">microsoft-edge/webview2/</a></li>
<li>ASP.NET Core 环境，下载地址：<a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer" target="_blank" rel="noopener noreferrer">runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer</a></li>
</ol>
<p><strong>网页版</strong>：</p>
<ol>
<li>ASP.NET Core 环境，下载地址：<a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer" target="_blank" rel="noopener noreferrer">runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer</a></li>
</ol>
<h3 id="直接下载安装包" tabindex="-1"><a class="header-anchor" href="#直接下载安装包"><span>直接下载安装包</span></a></h3>
<p><a href="https://gitee.com/galensgan/UZonMail/releases/latest" target="_blank" rel="noopener noreferrer">UZonMail-gitee</a></p>
<p>或者加 QQ 群 877458612 在群文件下载 (更新速度更快)</p>
<h3 id="自己手动编译" tabindex="-1"><a class="header-anchor" href="#自己手动编译"><span>自己手动编译</span></a></h3>
<ol>
<li>
<p>克隆仓库，切换到 <code v-pre>master</code> 分支；</p>
</li>
<li>
<p>使用命令行进入到项目根目录，执行 <code v-pre>./build.ps1</code> 开始编译，编译结果在 <code v-pre>build</code> 目录中。</p>
<p>编译成功截图：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240616124656131.png" alt="image-20240616124656131"></p>
</li>
</ol>
<blockquote>
<p>手动编译时，会自动检测环境，若没有相关环境，请根据提示进行安装。</p>
</blockquote>
<h3 id="使用" tabindex="-1"><a class="header-anchor" href="#使用"><span>使用</span></a></h3>
<p>本节只讲述通过安装包的使用方式。</p>
<ol>
<li>下载安装包</li>
<li>安装对应方式所需要的环境</li>
</ol>
<p>直接使用：</p>
<p>解压后，直接打开文件 <code v-pre>UzonMailDesktop.exe</code> 即可开始使用</p>
<p>网页端使用：</p>
<ol>
<li>首先启动服务：打开 <code v-pre>UzonMailDesktop.exe</code> 或 <code v-pre>service/UZonMailService.exe</code> 文件</li>
<li>在浏览器中输入地址：<code v-pre>http://localhost:22345</code> 打开网页</li>
</ol>
<h2 id="🍒发件步骤" tabindex="-1"><a class="header-anchor" href="#🍒发件步骤"><span>🍒发件步骤</span></a></h2>
<ol>
<li>添加发件箱（已添加请忽略）</li>
<li>添加收件箱（已添加请忽略）</li>
<li>导入所需模板（已添加请忽略）</li>
<li>打开【新建发件】，输入主题 --&gt; 选择收件人 --&gt; 选择模板 --&gt; 选择数据 --&gt;预览确认发件数量和模板正确性 --&gt; 退出预览 --&gt;点击【发送】</li>
<li>如果提示发送失败，转到【发件历史】，进行重发</li>
</ol>
<h3 id="邮件经常进发件箱怎么办" tabindex="-1"><a class="header-anchor" href="#邮件经常进发件箱怎么办"><span>邮件经常进发件箱怎么办</span></a></h3>
<p>可以使用 <a href="https://www.mail-tester.com/" target="_blank" rel="noopener noreferrer">Newsletters spam test by mail-tester.com</a> 检查待发邮件的内容。</p>
<p><img src="https://cdn.jsdelivr.net/gh/GalensGan/objects-storage/images/image-20220428140629151.png" alt="image-20220428140629151"></p>
<h2 id="🥑功能介绍" tabindex="-1"><a class="header-anchor" href="#🥑功能介绍"><span>🥑功能介绍</span></a></h2>
<h3 id="功能概览" tabindex="-1"><a class="header-anchor" href="#功能概览"><span>功能概览</span></a></h3>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614121857800.png" alt="image-20240614121857800"></p>
<p>软件主要由以下功能模块组成：</p>
<ul>
<li>
<p>首页</p>
<p>数据统计展示</p>
</li>
<li>
<p>邮箱管理</p>
<p>管理发件箱、收件箱</p>
</li>
<li>
<p>模板管理</p>
<p>管理正文模板</p>
</li>
<li>
<p>发件管理</p>
<p>新建发件和历史发件查询</p>
</li>
<li>
<p>系统设置</p>
<ul>
<li>
<p>用户管理</p>
<p>增加其它使用用户</p>
</li>
<li>
<p>基础设置</p>
<p>发件相关的全局设置</p>
</li>
<li>
<p>代理管理</p>
<p>配置发件代理</p>
</li>
</ul>
</li>
<li>
<p>支持作者</p>
<p>赞助页面</p>
</li>
<li>
<p>使用说明</p>
<p>帮助页</p>
</li>
</ul>
<p>下面开始按模块进行详细介绍。</p>
<h3 id="登陆界面" tabindex="-1"><a class="header-anchor" href="#登陆界面"><span>登陆界面</span></a></h3>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/uzon-mail-login-2.png" alt="uzon-mail-login-2"></p>
<p>默认用户名和密码为：admin/admin1234</p>
<h3 id="首页" tabindex="-1"><a class="header-anchor" href="#首页"><span>首页</span></a></h3>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614231957130.png" alt="image-20240614231957130"></p>
<p>首页主要展示的内容有：</p>
<ul>
<li>发件箱数量直方图</li>
<li>收件箱数量直方图</li>
<li>每月发件量折线图</li>
</ul>
<h3 id="系统设置" tabindex="-1"><a class="header-anchor" href="#系统设置"><span>系统设置</span></a></h3>
<h4 id="用户管理" tabindex="-1"><a class="header-anchor" href="#用户管理"><span>用户管理</span></a></h4>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612122713293.png" alt="image-20240612122713293"></p>
<p>系统默认的用户名为 admin，默认密码为 admin1234，这是一个管理员账号，该账号具有【管理用户】的权限。</p>
<p>【用户管理】模块用于增加不同的用户。桌面版本的多用户功能仅限本机使用，若要多人同时使用，则需要使用服务器版本。</p>
<p>服务器版本可联系作者获取。</p>
<p><strong>新增用户：</strong></p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612123329057.png" alt="image-20240612123329057"></p>
<p>单击左上角的新增，即可新增用户。</p>
<p>用户新增完成后，可对用户进行操作，比如重置密码，删除等</p>
<p>重置后的密码为：<code v-pre>uzonmail123</code>，在重置时会有提示。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612123429178.png" alt="image-20240612123429178"></p>
<p><strong>修改密码和头像：</strong></p>
<p>可以通过右上角的个人信息界面对头像和密码进行修改。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612125131168.png" alt="image-20240612125131168"></p>
<h4 id="基础设置" tabindex="-1"><a class="header-anchor" href="#基础设置"><span>基础设置</span></a></h4>
<p>基础设置中，主要设置全局的发件间隔，最大发件量等。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612125859579.png" alt="image-20240612125859579"></p>
<ul>
<li>
<p>单个邮箱每日最大发件量</p>
<p>控制单个邮箱每日发件的总数，避免因发件数量超过每个邮件服务提供商规定的每日发件量，从而导致发件失败。</p>
<p>为 0 时表示不限制</p>
</li>
<li>
<p>单个发件箱最小(最大)发件间隔</p>
<p>单位：秒</p>
<p>为了避免因频繁发送邮件而导致被服务器认为是垃圾邮箱，所以，发送两封邮件之间需要有一定的时间间隔，为了使得发送时间间隔具有不规律性，用间隔范围来进行控制：</p>
<p>实际发件间隔值 = 最小值 + （0，1）之间的随机数*（最大值-最小值）</p>
<p>最大值小于等于最小值时，表示不限制</p>
</li>
<li>
<p>合并发件最大数量</p>
<p>当同时向多个收件箱发送相同内容时，可以将收件箱合并成一封邮件发送，这个参数即控制合并的最大数量。</p>
<p>每个邮件服务商允许的最大数量不一样，最大数包含抄送和密送的数量</p>
<p>为 0 时，表示不合并</p>
</li>
</ul>
<h4 id="代理管理" tabindex="-1"><a class="header-anchor" href="#代理管理"><span>代理管理</span></a></h4>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612131312091.png" alt="image-20240612131312091"></p>
<p>代理管理模块主要针对使用国外邮箱的情况，允许针对某一类或者某个发件箱指定代理。</p>
<p>该功能一般用于服务器部署的情况，本机使用时，可以打开全局代理。</p>
<p><strong>新增代理</strong>：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612130111084.png" alt="image-20240612130111084"></p>
<p>新增代理参数说明：</p>
<ul>
<li>
<p>名称</p>
<p>必填项。在发件箱界面，可通过名称选择指定代理</p>
</li>
<li>
<p>代理地址</p>
<p>代理的地址、用户名和密码。格式为：<code v-pre>协议:\\username:password@host</code>，示例：</p>
<ol>
<li>完整格式：<code v-pre>http:\\admin:admin1234@127.0.0.1:7890</code></li>
<li>无密码格式: <code v-pre>http:\\127.0.0.1:7890</code></li>
<li>其它协议：<code v-pre>socket5:\\127.0.0.1:7890</code></li>
</ol>
<p>目前支持的协议有：<code v-pre>http、https、socks4、socks4a、socket5</code></p>
</li>
<li>
<p>匹配规则</p>
<p>若发件箱没有指定代理，则会从代理管理的列表中根据规则自动匹配，若匹配到，则使用。</p>
<p>该规则的语法为正则表达式</p>
<p><code v-pre>.*</code> 表示所有的邮件都匹配</p>
</li>
<li>
<p>优先级</p>
<p>规则匹配的优先级</p>
</li>
<li>
<p>是否共享</p>
<p>共享后，系统内所有人都可使用这个代理</p>
</li>
</ul>
<blockquote>
<p>代理安全提示</p>
<p>代理是明文存储在服务器中的，因此管理员可以查看代理的信息，可能会造成代理泄露风险，请谨慎添加个人代理</p>
</blockquote>
<h3 id="邮箱管理" tabindex="-1"><a class="header-anchor" href="#邮箱管理"><span>邮箱管理</span></a></h3>
<h4 id="发件箱" tabindex="-1"><a class="header-anchor" href="#发件箱"><span>发件箱</span></a></h4>
<p>【发件箱】模块用于管理发件人信息。下面列出在使用中需要注意的功能进行说明：</p>
<p><strong>组管理：</strong></p>
<p>增加发件箱时，必须先建立发件组。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612131552355.png" alt="image-20240612131552355"></p>
<p>在 &quot;发件箱&quot; 上右键，可新增发件组。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612131711338.png" alt="image-20240612131711338"></p>
<p>新增时，&quot;序号&quot; 表示发件组的排序号，仅用于排序。</p>
<p>当新建组完成后，可以在组名上右键，对组进行管理</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612131916319.png" alt="image-20240612131916319"></p>
<p><strong>新增发件箱</strong>：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240612132056895.png" alt="image-20240612132056895"></p>
<p>本软件采用的是 SMTP 协议发件，因此发件时，需要将自己的邮箱开通 SMTP 服务，可自行查阅资料。以下对一些重要参数说明：</p>
<ul>
<li>
<p>发件人名称</p>
<p>若有，当发给对方后，不显示邮箱，而是显示名称</p>
</li>
<li>
<p>smtp 密码</p>
<p>发件采用的 SMTP 服务器，所以，它的密码并不是邮箱的密码，而是登陆邮箱后，自己申请的 SMTP 密码。</p>
<p>比如，163邮箱 SMTP 密码获取方式如下：https://www.yisu.com/zixun/97973.html</p>
</li>
<li>
<p>smtp 地址</p>
<p>smtp 的地址，每种类型的地址不一样，可百度查找</p>
</li>
<li>
<p>smtp 端口</p>
<p>该端口与是否【启用 SSL】有关，默认为 25，若启动 SSL，一般为 465，需要自动查找确认。</p>
</li>
<li>
<p>启用 SSL</p>
<p>是在发件时，采用 SSL 加密，打开这个可以提升发件的安全性。</p>
</li>
<li>
<p>代理</p>
<p>若有需要，可以为其指定代理，可用代理候选项在【代理管理】中定义。</p>
</li>
</ul>
<blockquote>
<p>密码安全提示</p>
<p>服务器没有直接存储 smtp 密码，而是存储了通过密钥加密后的密文，密钥由前端生成，当有需要时，由前端传递给后端解密。</p>
<p>因此，即使数据库泄露了，也不会造成 smtp 的密码被盗的问题</p>
</blockquote>
<p><strong>从EXCEL导入：</strong></p>
<p>通过【导入】功能可批量从 Excel 中导入发件箱。</p>
<p>可以通过【模板】按钮下载导入模板。</p>
<p>在使用批量添加发件人功能时，Excel 表中第一行为表头，必须包含 <code v-pre>smtp邮箱</code>、<code v-pre>smtp密码</code>、<code v-pre>smtp地址</code>、<code v-pre>smtp端口</code>  列。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614123302248.png" alt="image-20240614123302248"></p>
<h4 id="收件箱" tabindex="-1"><a class="header-anchor" href="#收件箱"><span>收件箱</span></a></h4>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614124814856.png" alt="image-20240614124814856"></p>
<p>该模块主要用于对收件箱的分组和管理，使用方式、注意要点与发件人一致。</p>
<p>收件箱只需要姓名和邮箱即可，姓名是可选的。</p>
<h3 id="模板管理" tabindex="-1"><a class="header-anchor" href="#模板管理"><span>模板管理</span></a></h3>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614125056768.png" alt="image-20240614125056768"></p>
<p>在【正文模板】用于管理用户下的所有模板，它是 html 格式。</p>
<h4 id="新增模板" tabindex="-1"><a class="header-anchor" href="#新增模板"><span>新增模板</span></a></h4>
<p>使用两种方式进行添加：</p>
<ul>
<li>
<p>导入 HTML</p>
<p>先在外面用 html 定义好模板，然后通过上述中的【导入模板】功能将定义的模板导入到系统。对于自定义的 html 模板，要求其中的 css 必须为行内 css。可以通过 http://automattic.github.io/juice/ 自动将 html 文件中 css 转换成行内的 css。</p>
</li>
<li>
<p>直接编辑</p>
<p>通过单击【新增】按钮新增模板。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614125417120.png" alt="image-20240614125417120"></p>
</li>
</ul>
<h4 id="模板编辑" tabindex="-1"><a class="header-anchor" href="#模板编辑"><span>模板编辑</span></a></h4>
<p>通过单击模板名称或者在模板上右键，然后选择【编译】跳转到模板修改界面。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614125609679.png" alt="image-20240614125609679"></p>
<h4 id="模板变量" tabindex="-1"><a class="header-anchor" href="#模板变量"><span>模板变量</span></a></h4>
<p>在模板的编写过程中，可以使用双花括号（<code v-pre>{{变量名}}</code>）来标记为变量，在发件的过程中，程序会在数据中查找该变量，如果查找到，就会使用实际的数据将变量替换掉。</p>
<p>变量定义的格式是为：{{变量名}}。</p>
<blockquote>
<p>在发件中，模板也可以因发件人而异，需要在数据中增加 templateId 列来覆盖通用的模板。具体参考发件篇。</p>
</blockquote>
<h3 id="发件管理" tabindex="-1"><a class="header-anchor" href="#发件管理"><span>发件管理</span></a></h3>
<h4 id="新建发件" tabindex="-1"><a class="header-anchor" href="#新建发件"><span>新建发件</span></a></h4>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614125845479.png" alt="image-20240614125845479"></p>
<p>新建发件用于添加发件任务，通过不同的参数组合，它可以实现以下功能：</p>
<ol>
<li>
<p>一对一发件</p>
<p>一个发件箱，一个收件箱</p>
</li>
<li>
<p>一对多发件</p>
<p>一个发件箱，多个收件箱</p>
</li>
<li>
<p>多对多发件</p>
<p>多个发件箱，多个收件箱</p>
</li>
<li>
<p>主题变化、正文变化发件</p>
<p>同时支持主题和正文根据收件人不同而变化</p>
</li>
</ol>
<h5 id="主题" tabindex="-1"><a class="header-anchor" href="#主题"><span>主题</span></a></h5>
<p>发件的主题是必须的，主有两个作用：一是为邮件的主题，二是同一次发件将会归到一个发件历史记录中，该主题为历史记录组的名称。</p>
<p>多个主题使用英文分号（<code v-pre>;</code>）或者换行进行分隔。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614130827129.png" alt="image-20240614130827129"></p>
<p>若有多个主题，系统在发件时，会随机使用一个主题（若在数据中指定了主题，则会固定使用数据的主题）。</p>
<p>主题也支持变量声明，比如：{{日期}}-工资明细，<code v-pre>日期</code> 即为定义的变量，在发送邮件时，它将被替换成 Excel 表中的实际数据。</p>
<h5 id="模板" tabindex="-1"><a class="header-anchor" href="#模板"><span>模板</span></a></h5>
<p>模板相当于是正文的一个草稿，它可以让你快速发送正文，而不需要每次都在正文处输入。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614131034812.png" alt="image-20240614131034812"></p>
<p>可以选择多个模板。若有多个模板时，系统将随机选择一个模板来发件（若在数据中指定了模板，则会固定使用数据中的模板）</p>
<h5 id="正文" tabindex="-1"><a class="header-anchor" href="#正文"><span>正文</span></a></h5>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614131326894.png" alt="image-20240614131326894"></p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614225842011.png" alt="image-20240614225842011"></p>
<p>软件支持用户手动输入正文。</p>
<p>若用户指定了正文，则不会使用模板作为邮件正文。</p>
<p>正文的格式与模板一样，同样支持变量。</p>
<h5 id="发件人" tabindex="-1"><a class="header-anchor" href="#发件人"><span>发件人</span></a></h5>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614131606189.png" alt="image-20240614131606189"></p>
<p>单击发件人右侧的 + 号，选择发件人。</p>
<p>发件人允许有多个，若有多个发件人，发件时，将会把邮件随机给其中一个发件人发件。</p>
<p>在一次任务中，一封邮件只会被其中一个发件箱成功发送，不会多次发送。</p>
<h5 id="收件人" tabindex="-1"><a class="header-anchor" href="#收件人"><span>收件人</span></a></h5>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614131851665.png" alt="image-20240614131851665"></p>
<p>单击收件人右侧的 + 号，选择收件人。</p>
<p>单次发件任务中，允许添加多个收件人，每个收件都会收到一款邮件。</p>
<h5 id="抄送人" tabindex="-1"><a class="header-anchor" href="#抄送人"><span>抄送人</span></a></h5>
<p>若选择抄送人，每一封邮件都会抄送到每一个抄送人处。</p>
<h5 id="附件" tabindex="-1"><a class="header-anchor" href="#附件"><span>附件</span></a></h5>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614214517919.png" alt="image-20240614214517919"></p>
<p>若邮件中需要添加附件，可以在此添加附件。</p>
<p>允许添加多个附件，但是请注意，每封邮件都会携带相同的附件。</p>
<h5 id="数据" tabindex="-1"><a class="header-anchor" href="#数据"><span>数据</span></a></h5>
<p>支持数据发件功能是该软件的灵魂。通过导入数据，可以实现一次发件中，为不同的收件箱发送不同的内容。</p>
<p>当将鼠标聚焦在数据栏，右侧会出现下载模板的图标，单击该图标即可下载模板。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614215520531.png" alt="image-20240614215520531"></p>
<p>数据格式大致如下：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614220243117.png" alt="image-20240614220243117"></p>
<p><strong>数据的效果</strong>：</p>
<p>模板内容:</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614222544153.png" alt="image-20240614222544153"></p>
<p>模板赋予数据后的正文预览：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614222617857.png" alt="image-20240614222617857"></p>
<p><strong>数据的作用</strong>：</p>
<ol>
<li>为模板提供变量</li>
<li>快速实现精准的批量发送</li>
</ol>
<p><strong>数据中系统保留变量</strong>：</p>
<table>
<thead>
<tr>
<th>变量名</th>
<th>必须</th>
<th>描述</th>
</tr>
</thead>
<tbody>
<tr>
<td>inbox</td>
<td>是</td>
<td>指定收件邮箱。该字段必须存在，程序依靠该变量进行发件匹配。若为空，则该行数据无效。</td>
</tr>
<tr>
<td>inboxName</td>
<td>否</td>
<td>设置收件人名称。</td>
</tr>
<tr>
<td>subject</td>
<td>否</td>
<td>指定主题。若指定，则会忽略界面中输入的主题。</td>
</tr>
<tr>
<td>outbox</td>
<td>否</td>
<td>指定发件箱。若不指定，则使用用户在界面中选择的发件箱。<br />该发件箱必须是在【邮箱管理/发件箱】中添加的邮箱地址。其它邮箱则视为无效。</td>
</tr>
<tr>
<td>outboxName</td>
<td>否</td>
<td>设置发件箱名称。若不指定，则使用发件箱管理中的名称。</td>
</tr>
<tr>
<td>cc</td>
<td>否</td>
<td>指定抄送人。多个抄送人使用逗号分隔。</td>
</tr>
<tr>
<td>templateId</td>
<td>否</td>
<td>指定邮件的模板 Id。该模板 Id 可在模板管理中查看，是一个数字。<br />若不指定，则从用户选择的模板中随机取一个使用。</td>
</tr>
<tr>
<td>templateName</td>
<td>否</td>
<td>指定邮件的模板名称。该名称可在模板管理中查看。templateName 的优先级低于 templateId。当两者同时指定时，以 templateId 为主。<br />若不指定，则从用户选择的模板中随机取一个使用。</td>
</tr>
<tr>
<td>body</td>
<td>否</td>
<td>指定邮件的正文内容。该优先级大于 templateId 和 templateName。</td>
</tr>
</tbody>
</table>
<p><strong>数据的优先级</strong>：</p>
<ul>
<li>
<p>主题</p>
<p>【Excel数据/subject】  &gt; 【界面/主题】</p>
</li>
<li>
<p>正文</p>
<p>【Excel数据/body】 &gt; 【Excel数据/templateId】 &gt; 【Excel数据/templateName】 &gt; 【界面/正文】 &gt;【 界面/模板 】</p>
</li>
<li>
<p>发件人</p>
<p>【Excel数据/outbox 】&gt;【 界面/发件人】</p>
</li>
<li>
<p>收件人</p>
<p>【Excel数据/inbox 】&gt; 【界面/收件人】</p>
</li>
<li>
<p>抄送人</p>
<p>【Excel数据/cc】 &gt; 【界面/抄送人】</p>
</li>
<li>
<p>附件</p>
<p>附件目前无法通过数据指定</p>
</li>
</ul>
<h4 id="发件历史" tabindex="-1"><a class="header-anchor" href="#发件历史"><span>发件历史</span></a></h4>
<p>发件历史显示历次所发的所有邮件记录，一次发送任务为一条历史。</p>
<p>单击 ID 或者右键【详细】可查看具体发件项。</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614225346191.png" alt="image-20240614225346191"></p>
<p>发件明细：</p>
<p><img src="https://obs.uamazing.cn:52443/public/files/images/image-20240614231145776.png" alt="image-20240614231145776"></p>
<h2 id="🪺技术栈" tabindex="-1"><a class="header-anchor" href="#🪺技术栈"><span>🪺技术栈</span></a></h2>
<p>本系统采用前后端分离的模式进行开发。</p>
<p><strong>前端：</strong></p>
<p>typescript+vue3+quasar</p>
<p><strong>后端：</strong></p>
<p>C# + WPF +ASP.NET Core+webview2</p>
<p>使用 ASP.NET Core 实现服务端，桌面端使用 WPF 开发</p>
<h2 id="🍄服务器部署" tabindex="-1"><a class="header-anchor" href="#🍄服务器部署"><span>🍄服务器部署</span></a></h2>
<h3 id="简易版" tabindex="-1"><a class="header-anchor" href="#简易版"><span>简易版</span></a></h3>
<ol>
<li>安装 ASP.NET Core 环境，下载地址：<a href="https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer" target="_blank" rel="noopener noreferrer">runtime-aspnetcore-8.0.6-windows-hosting-bundle-installer</a></li>
<li>启动 <code v-pre>service/UZonMailService.exe</code></li>
</ol>
<p>至此，即可通过 <code v-pre>http://you-ip:22345</code> 进行访问了</p>
<h3 id="前后端分离部署" tabindex="-1"><a class="header-anchor" href="#前后端分离部署"><span>前后端分离部署</span></a></h3>
<p>到这一步，我相信您已经具备专业的知识，可以自己处理安装，此处只简述下前后端各部分内容：</p>
<ol>
<li>后端位于 <code v-pre>service</code> 目录</li>
<li>前端位于 <code v-pre>service/wwwroot</code> 目录</li>
</ol>
<p>您可以配置 nginx 进行反向代理，配置负载均衡</p>
<h3 id="前后端配置说明" tabindex="-1"><a class="header-anchor" href="#前后端配置说明"><span>前后端配置说明</span></a></h3>
<p>前端的配置位于 <code v-pre>service/wwwroot/app.config.ts</code> 文件中，若是对外提供服务，则需要修改后端接口地址，即 <code v-pre>baseUrl</code>：</p>
<div class="language-typescript line-numbers-mode" data-highlighter="prismjs" data-ext="ts" data-title="ts"><pre v-pre><code><span class="line"><span class="token keyword">export</span> <span class="token keyword">default</span> <span class="token punctuation">{</span></span>
<span class="line">  <span class="token comment">// 默认配置</span></span>
<span class="line">  <span class="token keyword">default</span><span class="token operator">:</span> <span class="token punctuation">{</span></span>
<span class="line">    baseUrl<span class="token operator">:</span> <span class="token string">'http://localhost:22345'</span><span class="token punctuation">,</span></span>
<span class="line">    api<span class="token operator">:</span> <span class="token string">'/api/v1'</span><span class="token punctuation">,</span></span>
<span class="line">    signalRHub<span class="token operator">:</span> <span class="token string">'/hubs/uzonMailHub'</span></span>
<span class="line">  <span class="token punctuation">}</span><span class="token punctuation">,</span></span>
<span class="line"></span>
<span class="line">  <span class="token comment">// 生产配置</span></span>
<span class="line">  prod<span class="token operator">:</span> <span class="token punctuation">{</span><span class="token punctuation">}</span><span class="token punctuation">,</span></span>
<span class="line"></span>
<span class="line">  <span class="token comment">// 开发配置</span></span>
<span class="line">  dev<span class="token operator">:</span> <span class="token punctuation">{</span><span class="token punctuation">}</span></span>
<span class="line"><span class="token punctuation">}</span> <span class="token keyword">as</span> Record<span class="token operator">&lt;</span><span class="token string">'default'</span> <span class="token operator">|</span> <span class="token string">'prod'</span> <span class="token operator">|</span> <span class="token string">'dev'</span><span class="token punctuation">,</span> <span class="token builtin">any</span><span class="token operator">></span></span>
<span class="line"></span></code></pre>
<div class="line-numbers" aria-hidden="true" style="counter-reset:line-number 0"><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div><div class="line-number"></div></div></div><p>后端的配置位于 <code v-pre>service/wwwroot/appsettings.json</code>，可以在这里配置：</p>
<ul>
<li>日志</li>
<li>授权密钥</li>
<li>系统参数</li>
<li>接口 Http 参数</li>
<li>Websocket 参数</li>
<li>数据库参数</li>
<li>初始用户参数</li>
<li>跨域</li>
<li>文件存储</li>
<li>定时任务</li>
</ul>
<p>请查看配置文件进行理解。</p>
<h2 id="🌵反馈与建议" tabindex="-1"><a class="header-anchor" href="#🌵反馈与建议"><span>🌵反馈与建议</span></a></h2>
<p>如果你在使用中发现了 bug, 或者对该软件有任何建议，都欢迎联系我，让我们将这款软件一起变得更优秀吧！</p>
<p>bug 反馈优先选择<a href="https://github.com/GalensGan/SendMultipleEmails/issues" target="_blank" rel="noopener noreferrer">Github Issues</a>，这样我能第一时间知道。</p>
<p>如果有紧急问题，请通过邮件联系。</p>
<p>QQ 和 QQ 群会不定时查看，所以不会很及时。</p>
<h2 id="🌶️联系方式" tabindex="-1"><a class="header-anchor" href="#🌶️联系方式"><span>🌶️联系方式</span></a></h2>
<p>QQ群：877458612</p>
<p>邮箱：260827400@qq.com</p>
<p>GitHub：<a href="https://github.com/GalensGan/UZonMail" target="_blank" rel="noopener noreferrer">GalensGan/UZonMail (github.com)</a></p>
<p>个人主页：<a href="https://galens.uamazing.cn" target="_blank" rel="noopener noreferrer">https://galens.uamazing.cn</a></p>
<h2 id="🍉致谢名单" tabindex="-1"><a class="header-anchor" href="#🍉致谢名单"><span>🍉致谢名单</span></a></h2>
<p>感谢老铁们对该软件的肯定，感谢大家的支持与鼓励！</p>
<p>感谢 QQ 用户 <code v-pre>来了来了</code>、<code v-pre>Me</code> 协助管理 QQ 群。</p>
<p>感谢以下用户的赞助（按先后时间排序）：</p>
<p>磊、鹏</p>
</div></template>


