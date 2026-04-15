import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { SuggestRecipesRequest, RecipeSuggestion } from "@/types/core";

export function useSuggestRecipes() {
  return useMutation({
    mutationFn: (data: SuggestRecipesRequest) =>
      coreClient.post<RecipeSuggestion[]>(
        "/api/recipe-assistant/suggest",
        data,
        true,
      ),
  });
}
