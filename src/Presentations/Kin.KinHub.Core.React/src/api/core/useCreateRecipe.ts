import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type { CreateRecipeRequest, RecipeResponse } from "@/types/core";

export function useCreateRecipe(recipeBookId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateRecipeRequest) =>
      coreClient.post<RecipeResponse>(
        `/api/recipe-books/${recipeBookId}/recipes`,
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
