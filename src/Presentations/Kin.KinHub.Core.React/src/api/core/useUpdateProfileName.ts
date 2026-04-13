import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyQueryKey } from "./useGetFamily";

export function useUpdateProfileName() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      familyId,
      memberId,
      name,
    }: {
      familyId: string;
      memberId: string;
      name: string;
    }) =>
      coreClient.put(
        `/api/families/${familyId}/members/${memberId}`,
        { name },
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: familyQueryKey });
    },
  });
}
