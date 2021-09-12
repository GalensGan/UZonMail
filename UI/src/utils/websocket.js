import WebSocketAsPromised from "websocket-as-promised";
import Channel from "chnl";
import { Notify } from "quasar";
import { getToken } from "./auth";

const wsUrl = "ws://127.0.0.1:22346";
const wsOption = {
  packMessage: data => JSON.stringify(data),
  unpackMessage: data => JSON.parse(data),
  // 附加数据,添加 token 进行验证
  // attach requestId to message as `id` field
  attachRequestId: (data, requestId) =>
    Object.assign({ id: requestId, token: getToken() }, data),
  extractRequestId: data => data && data.id // read requestId from message `id` field
};

// 初始化websocket
const ws = new WebSocketAsPromised(wsUrl, wsOption);

// 添加 chnl,用于保存频道消息
// https://vitalets.github.io/chnl/#channel
ws.$eventEmitter = new Channel.EventEmitter();

// 处理消息
function handleMessage(message) {
  // 此处 this 为空
  console.log("消息到达:", message);

  // 判断消息状态，如果消息状态码不是200,且没有取消拦截，就要报错
  if (message.status !== 200 && !message.ignoreError) {
    // 或使用配置对象：
    Notify.create({
      message: message.statusText,
      color: "negative",
      icon: "error"
    });

    // 重新登录
    if (message.status === 401) {
      // 跳转到重新登录界面
      window.location.replace(
        `${window.location.protocol}//${window.location.host}/login`
      );
    }
    return;
  }

  // 根据频道名称触发事件
  if (message.eventName) {
    ws.$eventEmitter.dispatch(message.eventName, message);
  }
}

// 添加常驻事件
ws.onUnpackedMessage.addListener(handleMessage);

try {
  ws.open().then(()=>{
    // 每次连接后，都要手动登陆，服务器才能记录通信 session    
    ws.sendRequest({
      name: 'Login',
      command: 'storeSession'
    })

    console.log("websocket 连接成功");
  })
 
} catch (e) {
  console.error(e);
}

export default ws;
