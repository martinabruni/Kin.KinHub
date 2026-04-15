import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipeBooksQueryKey } from "./useGetRecipeBooks";
import type { CreateRecipeBookRequest, RecipeBookResponse } from "@/types/core";

export function useCreateRecipeBook() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateRecipeBookRequest) =>
      coreClient.post<RecipeBookResponse>("/api/recipe-books", data, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: recipeBooksQueryKey });
    },
  });
}
