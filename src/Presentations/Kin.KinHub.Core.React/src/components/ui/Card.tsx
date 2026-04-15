import { cn } from "@/lib/cn";
import type { ReactNode } from "react";

interface CardProps {
  title?: string;
  children: ReactNode;
  className?: string;
  footer?: ReactNode;
  onClick?: () => void;
}

export function Card({
  title,
  children,
  className,
  footer,
  onClick,
}: CardProps) {
  return (
    <div
      onClick={onClick}
      role={onClick ? "button" : undefined}
      tabIndex={onClick ? 0 : undefined}
      onKeyDown={
        onClick
          ? (e) => {
              if (e.key === "Enter" || e.key === " ") {
                e.preventDefault();
                onClick();
              }
            }
          : undefined
      }
      className={cn(
        "rounded-xl border border-[var(--border)] bg-[var(--card)] p-6 shadow-sm",
        onClick && "cursor-pointer hover:bg-[var(--border)] transition-colors",
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
