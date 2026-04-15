import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipeBooksQueryKey } from "./useGetRecipeBooks";
import type { UpdateRecipeBookRequest, RecipeBookResponse } from "@/types/core";

export function useUpdateRecipeBook(id: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateRecipeBookRequest) =>
      coreClient.put<RecipeBookResponse>(`/api/recipe-books/${id}`, data, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: recipeBooksQueryKey });
    },
  });
}
