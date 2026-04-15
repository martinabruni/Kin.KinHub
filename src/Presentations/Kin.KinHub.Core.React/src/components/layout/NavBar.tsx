import { useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAuthStore } from "@/stores/authStore";
import { useProfileStore } from "@/stores/profileStore";
import { useUiStore } from "@/stores/uiStore";
import { useLogout } from "@/api/identity/useLogout";
import { useGetServices } from "@/api/core/useGetServices";
import { Button } from "@/components/ui/Button";
import { cn } from "@/lib/cn";

const serviceRoutes: Record<string, string> = {
  KinRecipe: "/recipe-books",
};

export function NavBar() {
  const { t, i18n } = useTranslation();
  const user = useAuthStore((s) => s.user);
  const refreshToken = useAuthStore((s) => s.refreshToken);
  const logout = useAuthStore((s) => s.logout);
  const clearProfile = useProfileStore((s) => s.clearProfile);
  const { selectedProfile } = useProfileStore();
  const { theme, toggleTheme, setLanguage } = useUiStore();
  const navigate = useNavigate();
  const logoutMutation = useLogout();
  const [menuOpen, setMenuOpen] = useState(false);
  const [servicesMenuOpen, setServicesMenuOpen] = useState(false);
  const { data: services } = useGetServices();

  const filteredServices = (services ?? []).filter(
    (s) => !s.isAdminOnly || selectedProfile?.role === "admin",
  );

  function handleLogout() {
    setMenuOpen(false);
    if (refreshToken) {
      logoutMutation.mutate(
        { refreshToken },
        {
          onSettled: () => {
            clearProfile();
            logout();
            navigate("/login", { replace: true });
          },
        },
      );
    } else {
      clearProfile();
      logout();
      navigate("/login", { replace: true });
    }
  }

  function toggleLanguage() {
    const next = i18n.language === "en" ? "it" : "en";
    i18n.changeLanguage(next);
    setLanguage(next);
  }

  const navLinkClass = ({ isActive }: { isActive: boolean }) =>
    cn(
      "rounded-md px-3 py-1.5 text-sm transition-colors",
      isActive
        ? "bg-[var(--border)] text-[var(--fg)] font-semibold"
        : "text-[var(--fg)] hover:bg-[var(--border)]",
    );

  const commonNavLinks = (
    <>
      <NavLink
        to="/family"
        onClick={() => setMenuOpen(false)}
        className={navLinkClass}
      >
        {t("nav.family")}
      </NavLink>
      <NavLink
        to="/profile"
        onClick={() => setMenuOpen(false)}
        className={navLinkClass}
      >
        {t("nav.profileSettings")}
      </NavLink>
    </>
  );

  return (
    <header className="sticky top-0 z-10 border-b border-[var(--border)] bg-[var(--card)]">
      <div className="mx-auto flex max-w-4xl items-center justify-between px-4 py-3">
        {/* Logo */}
        <Link
          to="/"
          className="text-base font-bold tracking-tight text-[var(--accent)]"
        >
          KinHub
        </Link>

        {/* Desktop nav links */}
        <nav className="hidden items-center gap-1 sm:flex">
          {/* Kin Services dropdown */}
          <div
            className="relative"
            onMouseEnter={() => setServicesMenuOpen(true)}
            onMouseLeave={() => setServicesMenuOpen(false)}
          >
            <button
              onClick={() => navigate("/")}
              className="rounded-md px-3 py-1.5 text-sm text-[var(--fg)] transition-colors hover:bg-[var(--border)]"
            >
              {t("nav.kinServices")}
            </button>
            {servicesMenuOpen && filteredServices.length > 0 && (
              <div className="absolute left-0 top-full z-20 min-w-[160px] rounded-md border border-[var(--border)] bg-[var(--card)] py-1 shadow-lg">
                {filteredServices.map((service) => (
                  <button
                    key={service.id}
                    onClick={() => {
                      setServicesMenuOpen(false);
                      navigate(
                        serviceRoutes[service.name] ??
                          `/services/${service.id}`,
                      );
                    }}
                    className="block w-full px-4 py-2 text-left text-sm text-[var(--fg)] hover:bg-[var(--border)]"
                  >
                    {service.name}
                  </button>
                ))}
              </div>
            )}
          </div>
          {commonNavLinks}
        </nav>

        {/* Actions */}
        <div className="flex items-center gap-2">
          {/* Language toggle */}
          <button
            onClick={toggleLanguage}
            className="rounded-md px-2 py-1.5 text-xs font-medium text-[var(--muted)] hover:bg-[var(--border)]"
            aria-label="Toggle language"
          >
            {i18n.language === "en" ? "IT" : "EN"}
          </button>

          {/* Theme toggle */}
          <button
            onClick={toggleTheme}
            className="rounded-md p-1.5 text-[var(--muted)] hover:bg-[var(--border)]"
            aria-label={
              theme === "dark" ? t("theme.toggleLight") : t("theme.toggleDark")
            }
          >
            {theme === "dark" ? <SunIcon /> : <MoonIcon />}
          </button>

          {/* User + logout (desktop) */}
          {user && (
            <Button
              variant="ghost"
              size="sm"
              loading={logoutMutation.isPending}
              onClick={handleLogout}
              className="hidden sm:inline-flex"
            >
              {t("nav.logout")}
            </Button>
          )}

          {/* Hamburger (mobile only) */}
          <button
            className="rounded-md p-1.5 text-[var(--fg)] hover:bg-[var(--border)] sm:hidden"
            aria-label="Toggle menu"
            aria-expanded={menuOpen}
            onClick={() => setMenuOpen((o) => !o)}
          >
            <HamburgerIcon open={menuOpen} />
          </button>
        </div>
      </div>

      {/* Mobile dropdown */}
      {menuOpen && (
        <div className="border-t border-[var(--border)] bg-[var(--card)] px-4 py-3 sm:hidden">
          <nav className="flex flex-col gap-1">
            {/* Kin Services header + service items */}
            <button
              onClick={() => {
                setMenuOpen(false);
                navigate("/");
              }}
              className="rounded-md px-3 py-1.5 text-left text-sm text-[var(--fg)] transition-colors hover:bg-[var(--border)]"
            >
              {t("nav.kinServices")}
            </button>
            {filteredServices.map((service) => (
              <button
                key={service.id}
                onClick={() => {
                  setMenuOpen(false);
                  navigate(
                    serviceRoutes[service.name] ?? `/services/${service.id}`,
                  );
                }}
                className="rounded-md py-1.5 pl-6 text-left text-sm text-[var(--muted)] transition-colors hover:bg-[var(--border)] hover:text-[var(--fg)]"
              >
                {service.name}
              </button>
            ))}
            {commonNavLinks}
          </nav>
          {user && (
            <Button
              variant="ghost"
              size="sm"
              loading={logoutMutation.isPending}
              onClick={handleLogout}
              className="mt-2 w-full justify-start"
            >
              {t("nav.logout")}
            </Button>
          )}
        </div>
      )}
    </header>
  );
}

function SunIcon() {
  return (
    <svg
      className="h-4 w-4"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <circle cx="12" cy="12" r="5" strokeWidth="2" />
      <line
        x1="12"
        y1="1"
        x2="12"
        y2="3"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="12"
        y1="21"
        x2="12"
        y2="23"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="4.22"
        y1="4.22"
        x2="5.64"
        y2="5.64"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="18.36"
        y1="18.36"
        x2="19.78"
        y2="19.78"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="1"
        y1="12"
        x2="3"
        y2="12"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="21"
        y1="12"
        x2="23"
        y2="12"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="4.22"
        y1="19.78"
        x2="5.64"
        y2="18.36"
        strokeWidth="2"
        strokeLinecap="round"
      />
      <line
        x1="18.36"
        y1="5.64"
        x2="19.78"
        y2="4.22"
        strokeWidth="2"
        strokeLinecap="round"
      />
    </svg>
  );
}

function MoonIcon() {
  return (
    <svg
      className="h-4 w-4"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      <path
        strokeLinecap="round"
        strokeLinejoin="round"
        strokeWidth="2"
        d="M21 12.79A9 9 0 1111.21 3 7 7 0 0021 12.79z"
      />
    </svg>
  );
}

function HamburgerIcon({ open }: { open: boolean }) {
  return (
    <svg
      className="h-5 w-5"
      fill="none"
      stroke="currentColor"
      viewBox="0 0 24 24"
      aria-hidden="true"
    >
      {open ? (
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="2"
          d="M6 18L18 6M6 6l12 12"
        />
      ) : (
        <path
          strokeLinecap="round"
          strokeLinejoin="round"
          strokeWidth="2"
          d="M4 6h16M4 12h16M4 18h16"
        />
      )}
    </svg>
  );
}
