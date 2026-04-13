import { useNavigate, Navigate } from "react-router-dom";
import { useGetFamily } from "@/api/core/useGetFamily";
import { useProfileStore } from "@/stores/profileStore";
import { ApiError } from "@/lib/http/httpClient";
import { Spinner } from "@/components/ui/Spinner";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import type { FamilyMemberDto } from "@/types/core";

export function SelectProfilePage() {
  const navigate = useNavigate();
  const { selectProfile } = useProfileStore();
  const { data: family, isLoading, error, refetch } = useGetFamily();

  const is404 = error instanceof ApiError && error.status === 404;

  if (isLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-[var(--bg)]">
        <Spinner size="lg" className="text-[var(--accent)]" />
      </div>
    );
  }

  if (is404) {
    return <Navigate to="/" replace />;
  }

  if (error || !family) {
    return (
      <div className="flex min-h-screen flex-col items-center justify-center gap-4 bg-[var(--bg)]">
        <p className="text-sm text-[var(--danger)]">
          {error instanceof ApiError
            ? error.message
            : "Errore imprevisto. Riprova."}
        </p>
        <Button onClick={() => refetch()}>Riprova</Button>
      </div>
    );
  }

  function handleSelect(member: FamilyMemberDto) {
    const role = member.role === "admin" ? "admin" : "member";

    if (role === "admin") {
      navigate("/verify-admin", {
        state: {
          memberId: member.id,
          familyId: family!.familyId,
          memberName: member.name,
        },
      });
    } else {
      selectProfile({
        id: member.id,
        name: member.name,
        role: "member",
        familyId: family!.familyId,
      });
      navigate("/", { replace: true });
    }
  }

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-8 bg-[var(--bg)] px-4 py-12">
      <h1 className="text-3xl font-bold text-[var(--fg)]">Chi sei?</h1>
      <p className="text-sm text-[var(--muted)]">{family.name}</p>

      <div className="flex flex-wrap justify-center gap-6">
        {family.members.map((member) => (
          <button
            key={member.id}
            onClick={() => handleSelect(member)}
            className="group flex flex-col items-center gap-3 rounded-xl border border-[var(--border)] bg-[var(--card)] p-6 shadow-sm transition hover:border-[var(--accent)] hover:shadow-md focus:outline-none focus:ring-2 focus:ring-[var(--accent)] w-40"
          >
            <div className="flex h-20 w-20 items-center justify-center rounded-full bg-[var(--accent)] text-2xl font-bold text-white">
              {member.name[0].toUpperCase()}
            </div>
            <span className="text-sm font-medium text-[var(--fg)]">
              {member.name}
            </span>
            <Badge
              variant={member.role === "admin" ? "admin" : "member"}
              label={member.role}
            />
          </button>
        ))}
      </div>
    </div>
  );
}
