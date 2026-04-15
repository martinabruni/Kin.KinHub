import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { AdaptRecipeRequest, RecipeAdaptationResult } from "@/types/core";

export function useAdaptRecipe() {
  return useMutation({
    mutationFn: (data: AdaptRecipeRequest) =>
      coreClient.post<RecipeAdaptationResult>(
        "/api/recipe-assistant/adapt",
        data,
        true,
      ),
  });
}
