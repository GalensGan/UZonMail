#!binbash

# 检查 docker
if ! command -v docker &> /dev/null
then
    echo "docker 未安装，请先安装 docker。"
    exit 1
fi

# 检查 docker compose
if ! command -v docker-compose &> /dev/null && ! command -v docker &> /dev/null
then
    echo "docker compose 未安装，请先安装 docker compose。"
    exit 1
fi

if [ ! -d 'service-linux-x64' ]; then
    echo "未找到 service-linux-x64 目录，请勿修改目录结构"
    exit 1
fi

cd service-linux-x64
# 开始构建
docker build -t uzon-mail:latest .
# 启动 docker-compose
cd ..
docker compose up -d