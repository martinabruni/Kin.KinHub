import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";

export function useUpdateAdminCode() {
  return useMutation({
    mutationFn: ({
      familyId,
      currentCode,
      newCode,
    }: {
      familyId: string;
      currentCode: string;
      newCode: string;
    }) =>
      coreClient.patch(
        `/api/families/${familyId}/admin-code`,
        { currentCode, newCode },
        true,
      ),
  });
}
