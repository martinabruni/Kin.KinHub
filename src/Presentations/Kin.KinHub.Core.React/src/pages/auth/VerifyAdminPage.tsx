import { useState, type FormEvent } from "react";
import { useNavigate, useLocation, Navigate } from "react-router-dom";
import { AuthLayout } from "@/components/layout/AuthLayout";
import { Card } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";
import { useVerifyAdminCode } from "@/api/core/useVerifyAdminCode";
import { useProfileStore } from "@/stores/profileStore";

interface LocationState {
  memberId: string;
  familyId: string;
  memberName: string;
}

export function VerifyAdminPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const state = location.state as LocationState | null;

  const { selectProfile } = useProfileStore();
  const [adminCode, setAdminCode] = useState("");
  const [error, setError] = useState<string | null>(null);

  const verifyMutation = useVerifyAdminCode(state?.familyId ?? "");

  if (!state?.memberId || !state?.familyId || !state?.memberName) {
    return <Navigate to="/select-profile" replace />;
  }

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);

    verifyMutation.mutate(
      { adminCode },
      {
        onSuccess: () => {
          selectProfile({
            id: state!.memberId,
            name: state!.memberName,
            role: "admin",
            familyId: state!.familyId,
          });
          navigate("/", { replace: true });
        },
        onError: (err) => {
          setError(err.message ?? "Codice admin non valido.");
        },
      },
    );
  }

  return (
    <AuthLayout>
      <Card title={`Benvenuto, ${state.memberName}`}>
        <p className="mb-4 text-sm text-[var(--muted)]">
          Inserisci il codice admin per continuare.
        </p>
        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            label="Codice Admin"
            type="password"
            value={adminCode}
            onChange={(e) => setAdminCode(e.target.value)}
            required
            autoComplete="off"
          />
          {error && <p className="text-sm text-[var(--danger)]">{error}</p>}
          <div className="flex gap-2">
            <Button type="submit" loading={verifyMutation.isPending}>
              Continua
            </Button>
            <Button
              type="button"
              variant="ghost"
              onClick={() => navigate("/select-profile")}
            >
              Indietro
            </Button>
          </div>
        </form>
      </Card>
    </AuthLayout>
  );
}
