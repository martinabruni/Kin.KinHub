import { Navigate, Outlet } from "react-router-dom";
import { useProfileStore } from "@/stores/profileStore";

export function AdminRoute() {
  const { selectedProfile } = useProfileStore();

  if (selectedProfile?.role !== "admin") {
    return <Navigate to="/select-profile" replace />;
  }

  return <Outlet />;
}
