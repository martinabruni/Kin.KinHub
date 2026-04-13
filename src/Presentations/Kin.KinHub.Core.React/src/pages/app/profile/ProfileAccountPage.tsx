import { useTranslation } from "react-i18next";
import { useUiStore } from "@/stores/uiStore";
import { Button } from "@/components/ui/Button";

export function ProfileAccountPage() {
  const { t } = useTranslation();
  const showSnackbar = useUiStore((s) => s.showSnackbar);

  function handleComingSoon() {
    showSnackbar(t("app.profile.account.comingSoon"));
  }

  return (
    <div className="flex flex-col gap-6">
      {/* Change email */}
      <div className="flex items-center justify-between rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <span className="text-sm text-[var(--fg)]">
          {t("app.profile.account.changeEmail")}
        </span>
        <Button size="sm" variant="secondary" onClick={handleComingSoon}>
          {t("app.profile.account.changeEmail")}
        </Button>
      </div>

      {/* Change password */}
      <div className="flex items-center justify-between rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <span className="text-sm text-[var(--fg)]">
          {t("app.profile.account.changePassword")}
        </span>
        <Button size="sm" variant="secondary" onClick={handleComingSoon}>
          {t("app.profile.account.changePassword")}
        </Button>
      </div>

      {/* Delete account */}
      <div className="flex items-center justify-between rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4">
        <span className="text-sm text-[var(--fg)]">
          {t("app.profile.account.deleteAccount")}
        </span>
        <Button size="sm" variant="danger" onClick={handleComingSoon}>
          {t("app.profile.account.deleteAccount")}
        </Button>
      </div>
    </div>
  );
}
