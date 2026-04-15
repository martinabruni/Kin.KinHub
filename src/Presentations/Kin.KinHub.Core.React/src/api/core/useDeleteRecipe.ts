import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";

export function useDeleteRecipe(recipeBookId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (recipeId: string) =>
      coreClient.delete<void>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}`,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: recipesQueryKey(recipeBookId),
      });
    },
  });
}
