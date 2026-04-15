import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgeIngredientsQueryKey } from "./useGetFridgeIngredients";
import type {
  UpdateFridgeIngredientRequest,
  FridgeIngredientResponse,
} from "@/types/core";

export function useUpdateFridgeIngredient(
  fridgeId: string,
  ingredientId: string,
) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: UpdateFridgeIngredientRequest) =>
      coreClient.put<FridgeIngredientResponse>(
        `/api/fridges/${fridgeId}/ingredients/${ingredientId}`,
        data,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: fridgeIngredientsQueryKey(fridgeId),
      });
    },
  });
}
