import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgesQueryKey } from "./useGetFridges";
import type { CreateFridgeRequest, FridgeResponse } from "@/types/core";

export function useCreateFridge() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateFridgeRequest) =>
      coreClient.post<FridgeResponse>("/api/fridges", data, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: fridgesQueryKey });
    },
  });
}
