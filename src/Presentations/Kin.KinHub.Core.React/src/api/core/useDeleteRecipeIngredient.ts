import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";

export function useDeleteRecipeIngredient(
  recipeBookId: string,
  recipeId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (ingredientId: string) =>
      coreClient.delete<void>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/ingredients/${ingredientId}`,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: recipesQueryKey(recipeBookId),
      });
    },
  });
}
