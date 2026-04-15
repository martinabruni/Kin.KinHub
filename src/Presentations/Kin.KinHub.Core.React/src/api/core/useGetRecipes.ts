import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { RecipeResponse } from "@/types/core";

export const recipesQueryKey = (recipeBookId: string) =>
  ["recipe-books", recipeBookId, "recipes"] as const;

export function useGetRecipes(recipeBookId: string) {
  return useQuery({
    queryKey: recipesQueryKey(recipeBookId),
    queryFn: () =>
      coreClient.get<RecipeResponse[]>(
        `/api/recipe-books/${recipeBookId}/recipes`,
        true,
      ),
    enabled: !!recipeBookId,
    staleTime: 30_000,
  });
}
