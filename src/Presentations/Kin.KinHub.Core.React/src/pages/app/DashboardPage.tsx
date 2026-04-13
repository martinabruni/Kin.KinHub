import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetFamily } from "@/api/core/useGetFamily";
import { useGetServices } from "@/api/core/useGetServices";
import { useCreateFamily } from "@/api/core/useCreateFamily";
import { useAuthStore } from "@/stores/authStore";
import { useProfileStore } from "@/stores/profileStore";
import { ApiError } from "@/lib/http/httpClient";

export function DashboardPage() {
  const { t } = useTranslation();
  const user = useAuthStore((s) => s.user);
  const { selectedProfile } = useProfileStore();
  const { data: family, isLoading, error } = useGetFamily();
  const { data: services, isLoading: servicesLoading } = useGetServices();
  const createFamily = useCreateFamily();
  const navigate = useNavigate();

  const [familyName, setFamilyName] = useState("");
  const [ownerName, setOwnerName] = useState(user?.displayName ?? "");
  const [adminCode, setAdminCode] = useState("");
  const [formError, setFormError] = useState<string | null>(null);
  const [showCreateForm, setShowCreateForm] = useState(false);

  const is404 = error instanceof ApiError && error.status === 404;

  function handleCreate(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    createFamily.mutate(
      { familyName, ownerProfileName: ownerName, adminCode },
      {
        onSuccess: () => {
          setShowCreateForm(false);
          setFamilyName("");
          setAdminCode("");
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
            <Input
              label="Codice Admin"
              type="password"
              value={adminCode}
              onChange={(e) => setAdminCode(e.target.value)}
              required
              autoComplete="off"
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

      {/* KinHub Services */}
      {family && (
        <section>
          <h2 className="mb-3 text-lg font-semibold text-[var(--fg)]">
            {t("app.dashboard.services")}
          </h2>
          {servicesLoading ? (
            <div className="flex justify-center py-6">
              <Spinner size="lg" className="text-[var(--accent)]" />
            </div>
          ) : (
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              {(services ?? [])
                .filter(
                  (s) => !s.isAdminOnly || selectedProfile?.role === "admin",
                )
                .map((service) => (
                  <button
                    key={service.id}
                    onClick={() => navigate(`/services/${service.id}`)}
                    className="rounded-xl border border-[var(--border)] bg-[var(--card)] px-5 py-4 text-left text-sm font-medium text-[var(--fg)] shadow-sm transition-colors hover:bg-[var(--border)]"
                  >
                    {service.name}
                  </button>
                ))}
              {(services ?? []).filter(
                (s) => !s.isAdminOnly || selectedProfile?.role === "admin",
              ).length === 0 && (
                <p className="text-sm text-[var(--muted)]">
                  {t("app.dashboard.noServices")}
                </p>
              )}
            </div>
          )}
        </section>
      )}
    </div>
  );
}
