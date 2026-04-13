import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyQueryKey } from "./useGetFamily";

export function useDeleteFamilyMember(familyId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ memberId }: { memberId: string }) =>
      coreClient.delete(`/api/families/${familyId}/members/${memberId}`, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: familyQueryKey });
    },
  });
}
