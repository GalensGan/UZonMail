import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/user/login',
    method: 'post',
    data
  })
}

export function getInfo(token) {
  return request({
    url: '/user/info',
    method: 'get',
    params: { token }
  })
}

export function logout() {
  return request({
    url: '/user/logout',
    method: 'put'
  })
}

// 更新用户的头像
export function updateUserAvatar(avatar, userId) {
  return request({
    url: '/user/avatar',
    method: 'put',
    data: {
      avatar,
      userId
    }
  })
}
