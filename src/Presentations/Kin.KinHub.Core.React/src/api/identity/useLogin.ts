import { useMutation } from "@tanstack/react-query";
import { identityClient } from "@/lib/http/identityClient";
import type { LoginRequest, LoginResponse } from "@/types/identity";

export function useLogin() {
  return useMutation({
    mutationFn: (data: LoginRequest) =>
      identityClient.post<LoginResponse>("/api/auth/login", data),
  });
}
