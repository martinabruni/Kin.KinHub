import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipesQueryKey } from "./useGetRecipes";

export function useDeleteRecipeStep(recipeBookId: string, recipeId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (stepId: string) =>
      coreClient.delete<void>(
        `/api/recipe-books/${recipeBookId}/recipes/${recipeId}/steps/${stepId}`,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: recipesQueryKey(recipeBookId),
      });
    },
  });
}
