import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type { UpdateRecipeRequest, RecipeResponse } from "@/types/core";

export function useUpdateRecipe(recipeBookId: string, recipeId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateRecipeRequest) =>
      coreClient.put<RecipeResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}`,
        data,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: recipesQueryKey(recipeBookId),
      });
    },
  });
}
