import { useMutation, useQueryClient } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";
import type { RefreshRequest } from "@/types/identity";

export function useLogout() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: RefreshRequest) =>
      identityClient.post<void>("/api/auth/logout", data),
    onSettled: () => {
      queryClient.clear();
    },
  });
}
