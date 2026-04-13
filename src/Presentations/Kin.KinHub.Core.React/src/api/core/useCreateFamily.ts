import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { familyQueryKey } from "./useGetFamily";
import type { CreateFamilyRequest, CreateFamilyResponse } from "@/types/core";

export function useCreateFamily() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateFamilyRequest) =>
      coreClient.post<CreateFamilyResponse>("/api/families", data, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: familyQueryKey });
    },
  });
}
