import type { ReactNode } from 'react'
import { createContext, useCallback, useContext } from 'react'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { toast } from 'sonner'
import { useTranslation } from 'react-i18next'
import { apiClient } from '@/api/apiClient'
import { useAuth } from '@/features/auth/AuthProvider'
import type { Family, FamilyMember } from '@/types'

interface FamilyContextValue {
  family: Family | undefined
  isLoading: boolean
  isAdmin: boolean
  updateName: (name: string) => Promise<void>
  addMember: (email: string) => Promise<void>
  updateMember: (memberId: string, role: 'Admin' | 'Member') => Promise<void>
  removeMember: (memberId: string) => Promise<void>
  verifyAdminCode: (code: string) => Promise<boolean>
  regenerateAdminCode: () => Promise<void>
  createFamily: (name: string) => Promise<void>
  leaveFamily: () => Promise<void>
  deleteFamily: () => Promise<void>
}

const FamilyContext = createContext<FamilyContextValue | null>(null)

export function FamilyProvider({ children }: { children: ReactNode }) {
  const { t } = useTranslation()
  const { user } = useAuth()
  const queryClient = useQueryClient()

  const { data: family, isLoading } = useQuery({
    queryKey: ['family'],
    queryFn: async () => {
      const { data } = await apiClient.get<Family>('/api/families')
      return data
    },
    enabled: !!user?.familyId,
    retry: false,
  })

  const invalidate = useCallback(() => {
    queryClient.invalidateQueries({ queryKey: ['family'] })
  }, [queryClient])

  const updateNameMutation = useMutation({
    mutationFn: (name: string) => apiClient.patch(`/api/families/${family!.id}`, { name }),
    onSuccess: () => { toast.success(t('family.updated')); invalidate() },
  })

  const addMemberMutation = useMutation({
    mutationFn: (email: string) =>
      apiClient.post(`/api/families/${family!.id}/members`, { email }),
    onSuccess: () => invalidate(),
  })

  const updateMemberMutation = useMutation({
    mutationFn: ({ memberId, role }: { memberId: string; role: FamilyMember['role'] }) =>
      apiClient.put(`/api/families/${family!.id}/members/${memberId}`, { role }),
    onSuccess: () => invalidate(),
  })

  const removeMemberMutation = useMutation({
    mutationFn: (memberId: string) =>
      apiClient.delete(`/api/families/${family!.id}/members/${memberId}`),
    onSuccess: () => { toast.success(t('family.memberRemoved')); invalidate() },
  })

  const regenerateCodeMutation = useMutation({
    mutationFn: () => apiClient.patch(`/api/families/${family!.id}/admin-code`),
    onSuccess: () => { toast.success(t('family.codeRegenerated')); invalidate() },
  })

  const createFamilyMutation = useMutation({
    mutationFn: (name: string) => apiClient.post('/api/families', { name }),
    onSuccess: () => invalidate(),
  })

  const leaveFamilyMutation = useMutation({
    mutationFn: () => apiClient.delete(`/api/families/${family!.id}/members/me`),
    onSuccess: () => { toast.success(t('family.left')); invalidate() },
  })

  const deleteFamilyMutation = useMutation({
    mutationFn: () => apiClient.delete(`/api/families/${family!.id}`),
    onSuccess: () => { toast.success(t('family.deleted')); invalidate() },
  })

  return (
    <FamilyContext.Provider
      value={{
        family,
        isLoading,
        isAdmin: user?.familyRole === 'Admin',
        updateName: async (name) => { await updateNameMutation.mutateAsync(name) },
        addMember: async (email) => { await addMemberMutation.mutateAsync(email) },
        updateMember: async (memberId, role) => { await updateMemberMutation.mutateAsync({ memberId, role }) },
        removeMember: async (memberId) => { await removeMemberMutation.mutateAsync(memberId) },
        verifyAdminCode: async (code) => {
          const { data } = await apiClient.post(
            `/api/families/${family!.id}/verify-admin-code`,
            { code },
          )
          return data.isValid ?? false
        },
        regenerateAdminCode: async () => { await regenerateCodeMutation.mutateAsync() },
        createFamily: async (name) => { await createFamilyMutation.mutateAsync(name) },
        leaveFamily: async () => { await leaveFamilyMutation.mutateAsync() },
        deleteFamily: async () => { await deleteFamilyMutation.mutateAsync() },
      }}
    >
      {children}
    </FamilyContext.Provider>
  )
}

export function useFamily() {
  const ctx = useContext(FamilyContext)
  if (!ctx) throw new Error('useFamily must be used within FamilyProvider')
  return ctx
}
