import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";

export function useUpdateUserEmail() {
  return useMutation({
    mutationFn: ({ newEmail }: { newEmail: string }) =>
      identityClient.put("/api/auth/me/email", { newEmail }, true),
  });
}
