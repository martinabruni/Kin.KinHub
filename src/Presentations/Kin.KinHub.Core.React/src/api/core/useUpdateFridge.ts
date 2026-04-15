import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgesQueryKey } from "./useGetFridges";
import type { UpdateFridgeRequest, FridgeResponse } from "@/types/core";

export function useUpdateFridge(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateFridgeRequest) =>
      coreClient.put<FridgeResponse>(`/api/fridges/${id}`, data, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: fridgesQueryKey });
    },
  });
}
