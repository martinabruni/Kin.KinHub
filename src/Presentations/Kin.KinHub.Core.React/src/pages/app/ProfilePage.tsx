import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Spinner } from "@/components/ui/Spinner";
import { useMe } from "@/api/identity/useMe";

export function ProfilePage() {
  const { t } = useTranslation();
  const { data: profile, isLoading } = useMe();

  if (isLoading) {
    return (
      <div className="flex justify-center pt-16">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">
        {t("app.profile.title")}
      </h1>

      <Card>
        <dl className="flex flex-col gap-4">
          <Row label={t("app.profile.email")} value={profile?.email} />
          <Row
            label={t("app.profile.displayName")}
            value={profile?.displayName ?? t("app.profile.noDisplayName")}
          />
          <Row label={t("app.profile.userId")} value={profile?.userId} mono />
        </dl>
      </Card>
    </div>
  );
}

function Row({
  label,
  value,
  mono = false,
}: {
  label: string;
  value?: string;
  mono?: boolean;
}) {
  return (
    <div>
      <dt className="text-xs font-medium uppercase tracking-wide text-[var(--muted)]">
        {label}
      </dt>
      <dd
        className={`mt-1 text-sm text-[var(--fg)] ${mono ? "font-mono" : ""}`}
      >
        {value ?? "—"}
      </dd>
    </div>
  );
}
