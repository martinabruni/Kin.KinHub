import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";
import type { RefreshRequest, LoginResponse } from "@/types/identity";

export function useRefresh() {
  return useMutation({
    mutationFn: (data: RefreshRequest) =>
      identityClient.post<LoginResponse>("/api/auth/refresh", data),
  });
}
