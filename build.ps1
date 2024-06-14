# 脚本说明
# 本脚本用于自动编译项目

# 严格模式,遇到错误即退出
Set-StrictMode -Version Latest

# 检测环境
Write-Host "开始检测环境..." -ForegroundColor Yellow

# 检测脚本位置是否正确：当前目录下是否有 ui-src 目录和 backend-src 目录
$scriptRoot = $PSScriptRoot
$subDirs = $("ui-src", "backend-src")
foreach ($subDir in $subDirs) {
    $dir = Join-Path -Path $scriptRoot -ChildPath $subDir
    if (-not (Test-Path -Path $dir -PathType Container)) {
        Write-Host "请在项目的根目录下执行当前脚本！"
        return
    }
}
Write-Host "脚本位置检测通过！" -ForegroundColor Green

# 检查是否有 yarn 环境
if (-not (Get-Command yarn -ErrorAction SilentlyContinue)) {
    Write-Host "请先安装 yarn 环境！" -ForegroundColor Red
    return
}
Write-Host "yarn 环境检测通过！" -ForegroundColor Green

# 检查是否有 node 环境
if (-not (Get-Command node -ErrorAction SilentlyContinue)) {
    Write-Host "请先安装 node 环境！" -ForegroundColor Red
    return
}
Write-Host "node 环境检测通过！" -ForegroundColor Green

# 检查是否有 dotnet 环境
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "请先安装 dotnet 环境！" -ForegroundColor Red
    return
}
Write-Host "dotnet 环境检测通过！" -ForegroundColor Green

# 检测 msbuild 环境
if (-not (Get-Command MSBuild -ErrorAction SilentlyContinue)) {
    Write-Host "请将 msbuild 添加到环境变量！" -ForegroundColor Red
    return
}
Write-Host "MSBuild 环境检测通过！" -ForegroundColor Green

# 检测 7z
if (-not (Get-Command 7z.exe -ErrorAction SilentlyContinue)) {
    Write-Host "7z 未安装，请从 https://www.7-zip.org/download.html 下载并安装" -ForegroundColor Red
    return
}
Write-Host "7z 环境检测通过！" -ForegroundColor Green


# 开始编译项目
Write-Host "开始编译项目..." -ForegroundColor Yellow

# 编译前端
Write-Host "前端编译中..." -ForegroundColor Yellow
$uiSrc = Join-Path -Path $scriptRoot -ChildPath "ui-src"
# 判断是否已经执行过 yarn install
$nodeModules = Join-Path -Path $uiSrc -ChildPath "node_modules"
if (-not (Test-Path -Path $nodeModules -PathType Container)) {
    Write-Host "开始安装依赖..." -ForegroundColor Yellow
    Set-Location -Path $uiSrc
    yarn install
    Write-Host "依赖安装完成！" -ForegroundColor Green
    Write-Host "开始编译..." -ForegroundColor Yellow
}

Set-Location -Path $uiSrc
yarn build
Write-Host "前端编译完成！" -ForegroundColor Green

# 编译后端
Write-Host "后端编译中..." -ForegroundColor Yellow
$backendSrc = Join-Path -Path $scriptRoot -ChildPath "backend-src"
$serviceSr = Join-Path -Path $backendSrc -ChildPath "UZonMailService"
# 使用 dotnet 编译
Set-Location -Path $serviceSr
$serviceDist = "$scriptRoot/build/service"
dotnet publish -c Release -o $serviceDist
# 创建 public 目录
New-Item -Path "$serviceDist/public" -ItemType Directory -Force
# 创建 wwwwroot 目录
New-Item -Path "$serviceDist/wwwroot" -ItemType Directory -Force
# 复制 Quartz/quartz-sqlite.sqlite3 到 Quartz 目录中
New-Item -Path "$serviceDist/Quartz" -ItemType Directory  -ErrorAction SilentlyContinue
Copy-Item -Path "$serviceSr/Quartz/quartz-sqlite.sqlite3" -Destination "$serviceDist/Quartz/quartz-sqlite.sqlite3" -Force
Write-Host "后端编译完成！" -ForegroundColor Green

# 编译桌面端
Write-Host "桌面端编译中..." -ForegroundColor Yellow
$desktopSrc = Join-Path -Path $backendSrc -ChildPath "UzonMailDesktop"
Set-Location -Path $desktopSrc
$desktopCsproj = Get-ChildItem -Path $desktopSrc -Filter "*.csproj" -Recurse | Select-Object -First 1
$desktopDist = "$scriptRoot/build/desktop"
# 若存在，则删除
if (Test-Path -Path $desktopDist -PathType Container) {
    Remove-Item -Path $desktopDist -Recurse -Force -ErrorAction SilentlyContinue
}
MSBuild.exe $desktopCsproj /t:Rebuild /p:Configuration=Release /p:DebugType=none /p:Platform=x64 /p:OutputPath=$desktopDist
Write-Host "桌面端编译完成！" -ForegroundColor Green

# 整合环境
Write-Host "合并编译结果..." -ForegroundColor Yellow
# 复制前端编译结果
$uiDist = Join-Path -Path $desktopDist -ChildPath "wwwroot"
New-Item -Path $uiDist -ItemType Directory -ErrorAction SilentlyContinue
Copy-Item -Path $uiSrc/dist/spa/* -Destination $uiDist -Recurse -Force

# 复制服务端
$svrDis = Join-Path -Path $desktopDist -ChildPath "service"
New-Item -Path $svrDis -ItemType Directory -ErrorAction SilentlyContinue
Copy-Item -Path $serviceDist/* -Destination $svrDis -Recurse -Force

# 读取 desktop.exe 的版本号
$desktopExePath = Join-Path -Path $desktopDist -ChildPath "UzonMailDesktop.exe"
$desktopVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($desktopExePath).FileVersion
# 生成文件路径
$zipDist = Join-Path -Path $scriptRoot -ChildPath "build\uzonmail-desktop-$desktopVersion.7z"

# 打包文件
7z a -t7z $zipDist "$desktopDist\*"

# 回到根目录
Set-Location -Path $scriptRoot
Write-Host "编译完成：$zipDist" -ForegroundColor Green
