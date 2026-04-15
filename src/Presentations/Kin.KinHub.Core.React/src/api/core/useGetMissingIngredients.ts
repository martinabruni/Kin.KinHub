import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type { MissingIngredientsResponse } from "@/types/core";

export function useGetMissingIngredients(
  recipeBookId: string,
  recipeId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (fridgeId: string) =>
      coreClient.post<MissingIngredientsResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/missing-ingredients?fridgeId=${fridgeId}`,
        undefined,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: recipesQueryKey(recipeBookId),
      });
    },
  });
}
