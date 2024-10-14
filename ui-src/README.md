# UZon Mail (uzon-mail)

A app for sending emails

## Install the dependencies
```bash
yarn install
# or
npm install
```

### Start the app in development mode (hot-code reloading, error reporting, etc.)
```bash
quasar dev
```


### Lint the files
```bash
yarn lint
# or
npm run lint
```



### Build the app for production
```bash
quasar build
```

### Customize the configuration
See [Configuring quasar.config.js](https://v2.quasar.dev/quasar-cli-vite/quasar-config-js).


## Docker 容器远程开发

``` bash
cd ui-src
docker compose up -d
# 第一次需要安装依赖
docker exec -it uzon-mail-dev yarn install && yarn run dev
# 第二次启动
docker exec -it uzon-mail-dev yarn run dev
```
