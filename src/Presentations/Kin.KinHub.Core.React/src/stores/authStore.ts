import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { LoginResponse } from '@/types/identity'

interface AuthState {
  user: { email: string; displayName?: string } | null
  accessToken: string | null
  refreshToken: string | null
  login: (response: LoginResponse) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      accessToken: null,
      refreshToken: null,
      login: (response) =>
        set({
          user: { email: response.email, displayName: response.displayName },
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
        }),
      logout: () => set({ user: null, accessToken: null, refreshToken: null }),
    }),
    { name: 'kinhub-auth' },
  ),
)
