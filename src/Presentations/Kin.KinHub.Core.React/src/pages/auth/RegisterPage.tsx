import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AuthLayout } from "@/components/layout/AuthLayout";
import { Card } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { useRegister } from "@/api/identity/useRegister";
import { useLogin } from "@/api/identity/useLogin";
import { useAuthStore } from "@/stores/authStore";
import { useProfileStore } from "@/stores/profileStore";

export function RegisterPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const storeLogin = useAuthStore((s) => s.login);
  const clearProfile = useProfileStore((s) => s.clearProfile);
  const registerMutation = useRegister();
  const loginMutation = useLogin();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [displayName, setDisplayName] = useState("");
  const [error, setError] = useState<string | null>(null);

  const isPending = registerMutation.isPending || loginMutation.isPending;

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    registerMutation.mutate(
      { email, password, displayName: displayName || undefined },
      {
        onSuccess: () => {
          // Auto-login after register
          loginMutation.mutate(
            { email, password },
            {
              onSuccess: (data) => {
                clearProfile();
                storeLogin(data);
                navigate("/", { replace: true });
              },
              onError: (err) => {
                setError(err.message ?? t("errors.generic"));
              },
            },
          );
        },
        onError: (err) => {
          setError(err.message ?? t("errors.generic"));
        },
      },
    );
  }

  return (
    <AuthLayout>
      <Card title={t("auth.register.title")}>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            label={t("auth.register.email")}
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
          />
          <Input
            label={t("auth.register.displayName")}
            type="text"
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            autoComplete="name"
          />
          <Input
            label={t("auth.register.password")}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="new-password"
          />
          {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
          <Button type="submit" loading={isPending}>
            {t("auth.register.submit")}
          </Button>
        </form>
        <p className="mt-4 text-center text-sm text-[var(--muted)]">
          {t("auth.register.haveAccount")}{" "}
          <Link to="/login" className="text-[var(--accent)] hover:underline">
            {t("auth.register.login")}
          </Link>
        </p>
      </Card>
    </AuthLayout>
  );
}
