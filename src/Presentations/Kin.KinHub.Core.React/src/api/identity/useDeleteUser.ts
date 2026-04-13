import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";

export function useDeleteUser() {
  return useMutation({
    mutationFn: () => identityClient.delete("/api/auth/me", true),
  });
}
