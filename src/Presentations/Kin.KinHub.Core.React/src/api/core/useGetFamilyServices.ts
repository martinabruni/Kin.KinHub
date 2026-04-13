import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { FamilyServiceDto } from "@/types/core";

export const familyServicesQueryKey = (familyId: string) =>
  ["familyServices", familyId] as const;

export function useGetFamilyServices(familyId: string) {
  return useQuery({
    queryKey: familyServicesQueryKey(familyId),
    queryFn: () =>
      coreClient.get<FamilyServiceDto[]>(
        `/api/services/family/${familyId}`,
        true,
      ),
    enabled: !!familyId,
  });
}
