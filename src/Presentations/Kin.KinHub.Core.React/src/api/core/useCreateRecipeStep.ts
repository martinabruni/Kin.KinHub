import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type { CreateRecipeStepRequest, RecipeStepResponse } from "@/types/core";

export function useCreateRecipeStep(recipeBookId: string, recipeId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateRecipeStepRequest) =>
      coreClient.post<RecipeStepResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/steps`,
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
