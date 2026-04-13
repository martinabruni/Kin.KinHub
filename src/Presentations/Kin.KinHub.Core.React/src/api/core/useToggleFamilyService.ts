import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyServicesQueryKey } from "./useGetFamilyServices";
import type {
  FamilyServiceDto,
  ToggleFamilyServiceRequest,
} from "@/types/core";

export function useToggleFamilyService(familyId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: ToggleFamilyServiceRequest) =>
      coreClient.post<FamilyServiceDto>(
        `/api/services/family/${familyId}/toggle`,
        data,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: familyServicesQueryKey(familyId),
      });
    },
  });
}
