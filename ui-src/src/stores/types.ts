export enum UserType {
  independent, // 独立的
  subUser, // 子账户
}

export enum UserStatus {
  normal, // 正常
  forbiddenLogin, // 禁止登陆
}

// 用户信息
export interface IUserInfo {
  userId: string
  userName: string,
  avatar: string,
  type: UserType,
  status: UserStatus,
  organizationId: number,
  departmentId: number
}