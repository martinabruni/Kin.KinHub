import { cn } from '@/lib/cn'
import type { InputHTMLAttributes } from 'react'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

export function Input({ label, error, id, className, ...props }: InputProps) {
  const inputId = id ?? label?.toLowerCase().replace(/\s+/g, '-')

  return (
    <div className="flex flex-col gap-1">
      {label && (
        <label
          htmlFor={inputId}
          className="text-sm font-medium text-[var(--fg)]"
        >
          {label}
        </label>
      )}
      <input
        id={inputId}
        className={cn(
          'rounded-md border border-[var(--border)] bg-[var(--card)] px-3 py-2 text-sm text-[var(--fg)]',
          'placeholder:text-[var(--muted)] focus:outline-none focus:ring-2 focus:ring-[var(--accent)]',
          'disabled:opacity-50',
          error && 'border-[var(--danger)] focus:ring-[var(--danger)]',
          className,
        )}
        {...props}
      />
      {error && <p className="text-xs text-[var(--danger)]">{error}</p>}
    </div>
  )
}
