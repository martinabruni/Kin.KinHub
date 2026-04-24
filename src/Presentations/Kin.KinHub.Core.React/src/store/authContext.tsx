import type { ReactNode } from 'react'
import { createContext, useCallback, useContext, useState } from 'react'
import type { User } from '@/types'

interface AuthContextValue {
  user: User | null
  accessToken: string | null
  setUser: (user: User | null) => void
  setTokens: (access: string, refresh: string) => void
  clearAuth: () => void
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthContextProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null)
  const [accessToken, setAccessToken] = useState<string | null>(
    () => localStorage.getItem('accessToken'),
  )

  const setTokens = useCallback((access: string, refresh: string) => {
    localStorage.setItem('accessToken', access)
    localStorage.setItem('refreshToken', refresh)
    setAccessToken(access)
  }, [])

  const clearAuth = useCallback(() => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    setAccessToken(null)
    setUser(null)
  }, [])

  return (
    <AuthContext.Provider
      value={{
        user,
        accessToken,
        setUser,
        setTokens,
        clearAuth,
        isAuthenticated: accessToken !== null,
      }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuthContext() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuthContext must be used within AuthContextProvider')
  return ctx
}
