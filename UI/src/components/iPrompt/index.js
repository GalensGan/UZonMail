import { Dialog, Notify } from "quasar";

function okCancle(title, message, options) {
  let initOptions = {
    title,
    message,
    ok: {
      dense: true,
      color: "warning"
    },
    cancel: {
      dense: true,
      color: "primary"
    },
    persistent: true
  };

  if (options) {
    Object.assign(initOptions, options);
  }

  const confirm = new Promise((resolve, reject) => {
    Dialog.create(initOptions)
      .onOk(() => {
        resolve(true);
      })
      .onCancel(() => {
        resolve(false);
      });
  });

  return confirm;
}

/**
 * 提示错误
 * @param {*} message
 */
function notifyError(message) {
  Notify.create({
    message,
    icon: "error",
    color: "negative",
    position: "top"
  });
}

/**
 * 提示成功
 * @param {*} message
 */
function notifySuccess(message) {
  Notify.create({
    message,
    icon: "done",
    color: "secondary",
    position: "top"
  });
}

export { okCancle, notifyError, notifySuccess };
