// Identity API types

export interface LoginRequest {
  email: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
  email: string
  displayName?: string
}

export interface RegisterRequest {
  email: string
  password: string
  displayName?: string
}

export interface RegisterResponse {
  userId: string
  email: string
}

export interface RefreshRequest {
  refreshToken: string
}

export interface UserProfileResponse {
  userId: string
  email: string
  displayName?: string
}
