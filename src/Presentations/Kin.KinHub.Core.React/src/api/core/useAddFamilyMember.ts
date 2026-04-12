import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyQueryKey } from "./useGetFamily";
import type {
  AddFamilyMemberRequest,
  AddFamilyMemberResponse,
} from "@/types/core";

export function useAddFamilyMember(familyId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: AddFamilyMemberRequest) =>
      coreClient.post<AddFamilyMemberResponse>(
        `/api/families/${familyId}/members`,
        data,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: familyQueryKey });
    },
  });
}
