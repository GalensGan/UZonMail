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
