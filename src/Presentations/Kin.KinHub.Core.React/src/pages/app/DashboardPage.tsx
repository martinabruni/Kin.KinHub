import { useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetFamily } from "@/api/core/useGetFamily";
import { useCreateFamily } from "@/api/core/useCreateFamily";
import { useAuthStore } from "@/stores/authStore";
import { ApiError } from "@/lib/http/httpClient";

export function DashboardPage() {
  const { t } = useTranslation();
  const user = useAuthStore((s) => s.user);
  const { data: family, isLoading, error } = useGetFamily();
  const createFamily = useCreateFamily();

  const [familyName, setFamilyName] = useState("");
  const [ownerName, setOwnerName] = useState(user?.displayName ?? "");
  const [formError, setFormError] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);

  const is404 = error instanceof ApiError && error.status === 404;

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    createFamily.mutate(
      { familyName, ownerProfileName: ownerName },
      {
        onSuccess: () => {
          setShowCreateForm(false);
          setFamilyName("");
        },
        onError: (err) => {
          setFormError(err.message ?? t("errors.generic"));
        },
      },
    );
  }

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
        {t("app.dashboard.welcome", {
          name: user?.displayName ?? user?.email ?? "",
        })}
      </h1>

      {/* No family yet */}
      {(is404 || !family) && !showCreateForm && (
        <Card>
          <p className="font-medium text-[var(--fg)]">
            {t("app.dashboard.noFamily")}
          </p>
          <p className="mt-1 text-sm text-[var(--muted)]">
            {t("app.dashboard.noFamilyHint")}
          </p>
          <Button className="mt-4" onClick={() => setShowCreateForm(true)}>
            {t("app.dashboard.createFamily")}
          </Button>
        </Card>
      )}

      {/* Create family form */}
      {showCreateForm && (
        <Card title={t("app.family.createTitle")}>
          <form onSubmit={handleCreate} className="flex flex-col gap-4">
            <Input
              label={t("app.family.familyName")}
              value={familyName}
              onChange={(e) => setFamilyName(e.target.value)}
              required
            />
            <Input
              label={t("app.family.ownerName")}
              value={ownerName}
              onChange={(e) => setOwnerName(e.target.value)}
              required
            />
            {formError && (
              <p className="text-sm text-[var(--danger)]">{formError}</p>
            )}
            <div className="flex gap-2">
              <Button type="submit" loading={createFamily.isPending}>
                {t("app.family.createSubmit")}
              </Button>
              <Button
                type="button"
                variant="ghost"
                onClick={() => setShowCreateForm(false)}
              >
                {t("app.family.cancel")}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {/* Family summary */}
      {family && (
        <Card title={t("app.dashboard.familySummary")}>
          <p className="text-lg font-semibold text-[var(--fg)]">
            {family.name}
          </p>
          <p className="mt-1 text-sm text-[var(--muted)]">
            {t("app.dashboard.members", { count: family.members.length })}
          </p>
          <Link
            to="/family"
            className="mt-4 inline-block text-sm text-[var(--accent)] hover:underline"
          >
            {t("app.dashboard.viewFamily")} →
          </Link>
        </Card>
      )}
    </div>
  );
}
