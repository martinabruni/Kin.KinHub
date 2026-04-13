import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { FamilyDetailResponse } from "@/types/core";

export const familyQueryKey = ["family"] as const;

export function useGetFamily() {
  return useQuery({
    queryKey: familyQueryKey,
    queryFn: () => coreClient.get<FamilyDetailResponse>("/api/families", true),
    staleTime: 30_000,
    retry: (failureCount, error) => {
      // Don't retry on 404 — means family doesn't exist yet
      if (
        error instanceof Error &&
        "status" in error &&
        (error as { status: number }).status === 404
      )
        return false;
      return failureCount < 2;
    },
  });
}
