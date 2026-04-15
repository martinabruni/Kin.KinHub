import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgesQueryKey } from "./useGetFridges";

export function useDeleteFridge() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) =>
      coreClient.delete<void>(`/api/fridges/${id}`, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: fridgesQueryKey });
    },
  });
}
