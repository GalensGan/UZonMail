import request from "@/utils/request";

export function getGroups(groupType) {
  return request({
    url: "/group",
    method: "get",
    params: {
      groupType
    }
  });
}

export function newGroup(data) {
  return request({
    url: "/group",
    method: "post",
    data
  });
}

export function deleteGroups(groupIds) {
  return request({
    url: "/groups",
    method: "delete",
    data: {
      groupIds
    }
  });
}

export function modifyGroup(groupId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/groups/${groupId}`,
    method: "put",
    data: {
      groupId,
      ...data
    }
  });
}

// 获取组下的邮箱
export function getEmails(groupId) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/groups/${groupId}/emails`,
    method: "get"
  });
}

// 在组中新建邮箱
export function newEmail(data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/groups/${data.groupId}/email`,
    method: "post",
    data
  });
}

// 批量新建邮箱
export function newEmails(groupId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/groups/${groupId}/emails`,
    method: "post",
    data
  });
}

// 删除邮箱
export function deleteEmail(id) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/emails/${id}`,
    method: "delete"
  });
}

// 删除组下面所有的邮箱
export function deleteEmails(groupId) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/groups/${groupId}/emails`,
    method: "delete"
  });
}

// 修改邮箱
export function modifyEmail(emailId, data) {
  // console.log("modifyGroup api:", groupId, data);
  return request({
    url: `/emails/${emailId}`,
    method: "put",
    data
  });
}
