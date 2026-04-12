import { cn } from '@/lib/cn'

interface BadgeProps {
  variant?: 'admin' | 'member'
  label: string
}

export function Badge({ variant = 'member', label }: BadgeProps) {
  return (
    <span
      className={cn(
        'inline-block rounded-full px-2.5 py-0.5 text-xs font-medium',
        variant === 'admin'
          ? 'bg-[var(--accent)] text-white'
          : 'bg-[var(--border)] text-[var(--muted)]',
      )}
    >
      {label}
    </span>
  )
}
