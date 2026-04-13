import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";
import type { RegisterRequest, RegisterResponse } from "@/types/identity";

export function useRegister() {
  return useMutation({
    mutationFn: (data: RegisterRequest) =>
      identityClient.post<RegisterResponse>("/api/auth/register", data),
  });
}
