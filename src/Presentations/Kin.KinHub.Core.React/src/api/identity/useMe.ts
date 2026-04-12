import { useQuery } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";
import { useAuthStore } from "@/stores/authStore";
import type { UserProfileResponse } from "@/types/identity";

export function useMe() {
  const accessToken = useAuthStore((s) => s.accessToken);

  return useQuery({
    queryKey: ["me"],
    queryFn: () =>
      identityClient.get<UserProfileResponse>("/api/auth/me", true),
    enabled: !!accessToken,
  });
}
