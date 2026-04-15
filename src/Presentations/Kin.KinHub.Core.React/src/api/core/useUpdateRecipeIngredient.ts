import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";
import type {
  UpdateRecipeIngredientRequest,
  RecipeIngredientResponse,
} from "@/types/core";

export function useUpdateRecipeIngredient(
  recipeBookId: string,
  recipeId: string,
  ingredientId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateRecipeIngredientRequest) =>
      coreClient.put<RecipeIngredientResponse>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/ingredients/${ingredientId}`,
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
