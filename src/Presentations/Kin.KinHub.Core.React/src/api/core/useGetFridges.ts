import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { FridgeResponse } from "@/types/core";

export const fridgesQueryKey = ["fridges"] as const;

export function useGetFridges() {
  return useQuery({
    queryKey: fridgesQueryKey,
    queryFn: () => coreClient.get<FridgeResponse[]>("/api/fridges", true),
    staleTime: 30_000,
  });
}
