import { useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { Card } from "@/components/ui/Card";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Spinner } from "@/components/ui/Spinner";
import { useGetFamily } from "@/api/core/useGetFamily";
import { useAddFamilyMember } from "@/api/core/useAddFamilyMember";
import { useDeleteFamilyMember } from "@/api/core/useDeleteFamilyMember";
import { ApiError } from "@/lib/http/httpClient";
import { useProfileStore } from "@/stores/profileStore";
import { useUiStore } from "@/stores/uiStore";

export function FamilyPage() {
  const { t } = useTranslation();
  const { data: family, isLoading, error } = useGetFamily();
  const is404 = error instanceof ApiError && error.status === 404;
  const { selectedProfile: profile } = useProfileStore();
  const isAdmin = profile?.role === "admin";
  const adminCount =
    family?.members.filter((m) => m.role.toLowerCase() === "admin").length ?? 0;

  const addMember = useAddFamilyMember(family?.familyId ?? "");
  const deleteMember = useDeleteFamilyMember(family?.familyId ?? "");
  const showSnackbar = useUiStore((s) => s.showSnackbar);
  const [showForm, setShowForm] = useState(false);
  const [memberName, setMemberName] = useState("");
  const [formError, setFormError] = useState<string | null>(null);

  function handleAdd(e: FormEvent) {
    e.preventDefault();
    setFormError(null);
    addMember.mutate(
      { name: memberName },
      {
        onSuccess: () => {
          setMemberName("");
          setShowForm(false);
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

  if (is404 || !family) {
    return (
      <div className="flex flex-col gap-4">
        <h1 className="text-2xl font-bold text-[var(--fg)]">
          {t("app.family.title")}
        </h1>
        <Card>
          <p className="text-[var(--muted)]">{t("app.dashboard.noFamily")}</p>
        </Card>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-6">
      <h1 className="text-2xl font-bold text-[var(--fg)]">{family.name}</h1>

      <Card title={t("app.family.members")}>
        <ul className="flex flex-col divide-y divide-[var(--border)]">
          {family.members.map((m) => (
            <li key={m.id} className="flex items-center justify-between py-3">
              <span className="text-sm font-medium text-[var(--fg)]">
                {m.name}
              </span>
              <div className="flex items-center gap-2">
                <Badge
                  variant={
                    m.role.toLowerCase() === "admin" ? "admin" : "member"
                  }
                  label={
                    m.role.toLowerCase() === "admin"
                      ? t("app.family.roleAdmin")
                      : t("app.family.roleMember")
                  }
                />
                {isAdmin &&
                  !(m.role.toLowerCase() === "admin" && adminCount <= 1) && (
                    <Button
                      size="sm"
                      variant="danger"
                      loading={deleteMember.isPending}
                      onClick={() =>
                        deleteMember.mutate(
                          { memberId: m.id },
                          {
                            onSuccess: () =>
                              showSnackbar(t("app.family.deleteMemberSuccess")),
                            onError: () =>
                              showSnackbar(t("app.family.deleteMemberError")),
                          },
                        )
                      }
                    >
                      {t("app.family.deleteMember")}
                    </Button>
                  )}
              </div>
            </li>
          ))}
        </ul>

        {/* Add member */}
        {isAdmin && !showForm && (
          <Button
            variant="secondary"
            size="sm"
            className="mt-4"
            onClick={() => setShowForm(true)}
          >
            + {t("app.family.addMember")}
          </Button>
        )}

        {isAdmin && showForm && (
          <form onSubmit={handleAdd} className="mt-4 flex flex-col gap-3">
            <Input
              label={t("app.family.memberName")}
              value={memberName}
              onChange={(e) => setMemberName(e.target.value)}
              required
              autoFocus
            />
            {formError && (
              <p className="text-sm text-[var(--danger)]">{formError}</p>
            )}
            <div className="flex gap-2">
              <Button type="submit" size="sm" loading={addMember.isPending}>
                {t("app.family.addMemberSubmit")}
              </Button>
              <Button
                type="button"
                size="sm"
                variant="ghost"
                onClick={() => {
                  setShowForm(false);
                  setMemberName("");
                }}
              >
                {t("app.family.cancel")}
              </Button>
            </div>
          </form>
        )}
      </Card>
    </div>
  );
}
