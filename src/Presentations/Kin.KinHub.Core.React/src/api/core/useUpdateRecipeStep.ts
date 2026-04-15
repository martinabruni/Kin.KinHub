import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type { UpdateRecipeStepRequest, RecipeStepResponse } from "@/types/core";

export function useUpdateRecipeStep(
  recipeBookId: string,
  recipeId: string,
  stepId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateRecipeStepRequest) =>
      coreClient.put<RecipeStepResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/steps/${stepId}`,
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
