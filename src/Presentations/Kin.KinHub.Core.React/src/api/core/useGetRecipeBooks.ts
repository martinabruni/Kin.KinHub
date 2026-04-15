import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { RecipeBookResponse } from "@/types/core";

export const recipeBooksQueryKey = ["recipe-books"] as const;

export function useGetRecipeBooks() {
  return useQuery({
    queryKey: recipeBooksQueryKey,
    queryFn: () =>
      coreClient.get<RecipeBookResponse[]>("/api/recipe-books", true),
    staleTime: 30_000,
  });
}
