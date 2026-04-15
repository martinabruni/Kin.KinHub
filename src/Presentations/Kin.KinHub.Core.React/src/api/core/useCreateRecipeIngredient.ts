import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type {
  CreateRecipeIngredientRequest,
  RecipeIngredientResponse,
} from "@/types/core";

export function useCreateRecipeIngredient(
  recipeBookId: string,
  recipeId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateRecipeIngredientRequest) =>
      coreClient.post<RecipeIngredientResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/ingredients`,
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
