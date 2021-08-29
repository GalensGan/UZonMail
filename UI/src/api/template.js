import request from "@/utils/request";

// 添加模板
export function newTemplate(name, imageUrl, html) {
  return request({
    url: "/template",
    method: "post",
    data: {
      name,
      imageUrl,
      html
    }
  });
}

// 删除模板
export function deleteTemplate(templateId) {
  return request({
    url: `/template/${templateId}`,
    method: "delete"
  });
}

// 获取当前用户下的所有模板
export function getTemplates() {
  return request({
    url: "/templates",
    method: "get"
  });
}
