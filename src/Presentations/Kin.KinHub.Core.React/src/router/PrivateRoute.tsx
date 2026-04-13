import { Navigate, Outlet } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/authStore";
import { useProfileStore } from "@/stores/profileStore";
import { AppLayout } from "@/components/layout/AppLayout";
import { ApiError } from "@/lib/http/httpClient";
import { familyQueryKey } from "@/api/core/useGetFamily";

export function PrivateRoute() {
  const accessToken = useAuthStore((s) => s.accessToken);
  const { selectedProfile } = useProfileStore();
  const queryClient = useQueryClient();

  if (!accessToken) {
    return <Navigate to="/login" replace />;
  }

  // If the family query already returned 404, the user has no family yet.
  // Don't redirect to /select-profile — let them land on the dashboard to create one.
  const familyQueryState = queryClient.getQueryState(familyQueryKey);
  const hasNoFamily =
    familyQueryState?.error instanceof ApiError &&
    familyQueryState.error.status === 404;

  if (!selectedProfile && !hasNoFamily) {
    return <Navigate to="/select-profile" replace />;
  }

  return (
    <AppLayout>
      <Outlet />
    </AppLayout>
  );
}
