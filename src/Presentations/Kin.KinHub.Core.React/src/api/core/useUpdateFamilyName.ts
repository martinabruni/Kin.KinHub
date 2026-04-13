import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyQueryKey } from "./useGetFamily";

export function useUpdateFamilyName() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ familyId, name }: { familyId: string; name: string }) =>
      coreClient.patch(`/api/families/${familyId}`, { name }, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: familyQueryKey });
    },
  });
}
