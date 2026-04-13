import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";

export function useUpdateUserPassword() {
  return useMutation({
    mutationFn: ({
      currentPassword,
      newPassword,
    }: {
      currentPassword: string;
      newPassword: string;
    }) =>
      identityClient.put(
        "/api/auth/me/password",
        { currentPassword, newPassword },
        true,
      ),
  });
}
