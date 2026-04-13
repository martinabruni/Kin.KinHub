import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useProfileStore } from "@/stores/profileStore";
import { cn } from "@/lib/cn";

export function ProfilePage() {
  const { t } = useTranslation();
  const { selectedProfile } = useProfileStore();
  const navigate = useNavigate();

  const linkClass = ({ isActive }: { isActive: boolean }) =>
    cn(
      "rounded-md px-3 py-2 text-sm font-medium transition-colors",
      isActive
        ? "bg-[var(--accent)] text-white"
        : "text-[var(--fg)] hover:bg-[var(--border)]",
    );

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">
        {t("nav.profileSettings")}
      </h1>

      <div className="flex flex-col gap-6 sm:flex-row sm:gap-8">
        {/* Side menu (desktop) / Tab bar (mobile) */}
        <nav className="flex shrink-0 flex-row gap-1 overflow-x-auto sm:w-48 sm:flex-col">
          <NavLink to="/profile" end className={linkClass}>
            {t("app.profile.menu.information")}
          </NavLink>
          {selectedProfile?.role === "admin" && (
            <NavLink to="/profile/account" className={linkClass}>
              {t("app.profile.menu.account")}
            </NavLink>
          )}
          <button
            onClick={() => navigate("/select-profile")}
            className="rounded-md px-3 py-2 text-left text-sm font-medium text-[var(--fg)] transition-colors hover:bg-[var(--border)]"
          >
            {t("app.profile.menu.changeProfile")}
          </button>
        </nav>

        {/* Content */}
        <div className="min-w-0 flex-1">
          <Outlet />
        </div>
      </div>
    </div>
  );
}
