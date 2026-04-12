import { cn } from "@/lib/cn";
import type { ReactNode } from "react";

interface CardProps {
  title?: string;
  children: ReactNode;
  className?: string;
  footer?: ReactNode;
}

export function Card({ title, children, className, footer }: CardProps) {
  return (
    <div
      className={cn(
        "rounded-xl border border-[var(--border)] bg-[var(--card)] p-6 shadow-sm",
        className,
      )}
    >
      {title && (
        <h2 className="mb-4 text-base font-semibold text-[var(--fg)]">
          {title}
        </h2>
      )}
      <div>{children}</div>
      {footer && (
        <div className="mt-4 border-t border-[var(--border)] pt-4">
          {footer}
        </div>
      )}
    </div>
  );
}
