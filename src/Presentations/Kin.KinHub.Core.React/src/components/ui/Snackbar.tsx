import { useEffect } from "react";
import { useUiStore } from "@/stores/uiStore";

export function Snackbar() {
  const snackbar = useUiStore((s) => s.snackbar);
  const hideSnackbar = useUiStore((s) => s.hideSnackbar);

  useEffect(() => {
    if (!snackbar) return;
    const timer = setTimeout(hideSnackbar, 4000);
    return () => clearTimeout(timer);
  }, [snackbar, hideSnackbar]);

  if (!snackbar) return null;

  return (
    <div
      key={snackbar.id}
      role="status"
      aria-live="polite"
      className="fixed bottom-4 left-4 z-50 max-w-sm rounded-lg border border-[var(--border)] bg-[var(--card)] px-4 py-3 text-sm text-[var(--fg)] shadow-lg"
    >
      {snackbar.message}
    </div>
  );
}
