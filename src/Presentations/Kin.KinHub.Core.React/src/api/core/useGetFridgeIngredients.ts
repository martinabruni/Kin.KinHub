import { useQuery } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import type { FridgeIngredientResponse } from "@/types/core";

export const fridgeIngredientsQueryKey = (fridgeId: string) =>
  ["fridges", fridgeId, "ingredients"] as const;

export function useGetFridgeIngredients(fridgeId: string) {
  return useQuery({
    queryKey: fridgeIngredientsQueryKey(fridgeId),
    queryFn: () =>
      coreClient.get<FridgeIngredientResponse[]>(
        `/api/fridges/${fridgeId}/ingredients`,
        true,
      ),
    enabled: !!fridgeId,
    staleTime: 30_000,
  });
}
