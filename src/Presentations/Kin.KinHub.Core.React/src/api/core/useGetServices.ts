import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { KinHubServiceDto } from "@/types/core";

export const servicesQueryKey = ["services"] as const;

export function useGetServices() {
  return useQuery({
    queryKey: servicesQueryKey,
    queryFn: () => coreClient.get<KinHubServiceDto[]>("/api/services", true),
  });
}
