import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { ParseRecipeRequest, RecipeAssistantRecipe } from "@/types/core";

export function useParseRecipe() {
  return useMutation({
    mutationFn: (data: ParseRecipeRequest) =>
      coreClient.post<RecipeAssistantRecipe | null>(
        "/api/recipe-assistant/parse",
        data,
        true,
      ),
  });
}
