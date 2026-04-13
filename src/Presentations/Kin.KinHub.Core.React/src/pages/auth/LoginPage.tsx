import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { AuthLayout } from "@/components/layout/AuthLayout";
import { Card } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { useLogin } from "@/api/identity/useLogin";
import { useAuthStore } from "@/stores/authStore";

export function LoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const login = useAuthStore((s) => s.login);
  const loginMutation = useLogin();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    loginMutation.mutate(
      { email, password },
      {
        onSuccess: (data) => {
          login(data);
          navigate("/select-profile", { replace: true });
        },
        onError: (err) => {
          setError(err.message ?? t("errors.generic"));
        },
      },
    );
  }

  return (
    <AuthLayout>
      <Card title={t("auth.login.title")}>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            label={t("auth.login.email")}
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
          />
          <Input
            label={t("auth.login.password")}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="current-password"
          />
          {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
          <Button type="submit" loading={loginMutation.isPending}>
            {t("auth.login.submit")}
          </Button>
        </form>
        <p className="mt-4 text-center text-sm text-[var(--muted)]">
          {t("auth.login.noAccount")}{" "}
          <Link to="/register" className="text-[var(--accent)] hover:underline">
            {t("auth.login.register")}
          </Link>
        </p>
      </Card>
    </AuthLayout>
  );
}
