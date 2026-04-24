import { Link } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { BookOpen, Grid2x2, Refrigerator, Sparkles, Users } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { Card, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { Separator } from '@/components/ui/separator'
import { Avatar, AvatarFallback } from '@/components/ui/avatar'
import { Badge } from '@/components/ui/badge'
import { useAuth } from '@/features/auth/AuthProvider'
import { apiClient } from '@/api/apiClient'
import type { Family, Service } from '@/types'
import { getInitials } from '@/lib/utils'

const serviceIcons: Record<string, React.ElementType> = {
  Recipes: BookOpen,
  Fridges: Refrigerator,
  'AI Assistant': Sparkles,
  Family: Users,
  Services: Grid2x2,
}

export function DashboardPage() {
  const { t } = useTranslation()
  const { user } = useAuth()

  const { data: family, isLoading: loadingFamily } = useQuery({
    queryKey: ['family'],
    queryFn: async () => {
      const { data } = await apiClient.get<Family>('/api/families')
      return data
    },
    enabled: !!user?.familyId,
    retry: false,
  })

  const { data: services, isLoading: loadingServices } = useQuery({
    queryKey: ['services', 'family', user?.familyId],
    queryFn: async () => {
      const { data } = await apiClient.get<Service[]>(`/api/services/family/${user!.familyId}`)
      return data
    },
    enabled: !!user?.familyId,
  })

  const displayName = user?.email?.split('@')[0] ?? 'there'
  const today = new Date().toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' })

  return (
    <div>
      <h1 className="text-3xl font-bold">{t('dashboard.greeting', { name: displayName })}</h1>
      <p className="text-muted-foreground text-sm mt-1">{today}</p>
      <Separator className="my-6" />

      {/* Family Card */}
      {loadingFamily ? (
        <Skeleton className="h-24 w-full rounded-xl mb-8" />
      ) : family ? (
        <Card className="mb-8 border-l-4 border-l-primary">
          <CardContent className="flex items-center gap-4 p-5">
            <Avatar className="w-12 h-12">
              <AvatarFallback className="text-lg bg-primary/20 text-primary">
                {getInitials(family.name)}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1">
              <div className="flex items-center gap-2">
                <span className="text-xl font-semibold">{family.name}</span>
                {user?.familyRole === 'Admin' && (
                  <Badge variant="secondary">Admin</Badge>
                )}
              </div>
              <p className="text-muted-foreground text-sm">
                {t('dashboard.members', { count: family.members?.length ?? 0 })}
              </p>
            </div>
            <Link to="/family" className="text-sm text-primary font-medium hover:underline">
              {t('dashboard.manageFamily')}
            </Link>
          </CardContent>
        </Card>
      ) : (
        <Card className="mb-8 bg-muted/40">
          <CardContent className="flex flex-col items-center gap-4 py-8">
            <p className="text-muted-foreground font-medium">{t('dashboard.noFamily.title')}</p>
            <div className="flex gap-3">
              <Button asChild size="sm">
                <Link to="/family">{t('dashboard.noFamily.create')}</Link>
              </Button>
              <Button asChild variant="outline" size="sm">
                <Link to="/family">{t('dashboard.noFamily.join')}</Link>
              </Button>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Services Grid */}
      <h2 className="text-lg font-semibold mb-4">{t('dashboard.yourServices')}</h2>
      {loadingServices ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {Array.from({ length: 3 }).map((_, i) => (
            <Skeleton key={i} className="h-32 rounded-xl" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {(services ?? [])
            .filter((s) => s.isEnabled)
            .map((service) => {
              const Icon = serviceIcons[service.name] ?? Grid2x2
              const serviceLink = service.name.toLowerCase().includes('recipe')
                ? '/recipe-books'
                : service.name.toLowerCase().includes('fridge')
                ? '/fridges'
                : service.name.toLowerCase().includes('ai')
                ? '/ai-assistant'
                : '/services'
              return (
                <Card
                  key={service.id}
                  className="hover:shadow-md hover:border-primary/40 transition-all"
                >
                  <CardContent className="p-5">
                    <Icon className="w-8 h-8 text-primary mb-2" />
                    <p className="font-semibold">{service.name}</p>
                    <p className="text-muted-foreground text-sm mt-1">{service.description}</p>
                    <Link
                      to={serviceLink}
                      className="text-sm text-primary font-medium hover:underline mt-3 inline-block"
                    >
                      {t('dashboard.open')}
                    </Link>
                  </CardContent>
                </Card>
              )
            })}
        </div>
      )}

      <p className="text-muted-foreground text-sm italic mt-8">{t('dashboard.recentActivity')}</p>
    </div>
  )
}
