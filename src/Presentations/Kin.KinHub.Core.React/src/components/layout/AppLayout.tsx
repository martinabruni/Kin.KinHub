import type { ReactNode } from "react";
import { NavBar } from "./NavBar";
import { Snackbar } from "@/components/ui/Snackbar";

interface AppLayoutProps {
  children: ReactNode;
}

export function AppLayout({ children }: AppLayoutProps) {
  return (
    <div className="min-h-screen bg-[var(--bg)]">
      <NavBar />
      <main className="mx-auto max-w-4xl px-4 py-8">{children}</main>
      <Snackbar />
    </div>
  );
}
