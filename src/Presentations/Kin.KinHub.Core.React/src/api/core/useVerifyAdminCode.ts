import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { VerifyAdminCodeRequest } from "@/types/core";

export function useVerifyAdminCode(familyId: string) {
  return useMutation({
    mutationFn: (data: VerifyAdminCodeRequest) =>
      coreClient.post<boolean>(
        `/api/families/${familyId}/verify-admin-code`,
        data,
        true,
      ),
  });
}
