import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgeIngredientsQueryKey } from "./useGetFridgeIngredients";
import type {
  CreateFridgeIngredientRequest,
  FridgeIngredientResponse,
} from "@/types/core";

export function useCreateFridgeIngredient(fridgeId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CreateFridgeIngredientRequest) =>
      coreClient.post<FridgeIngredientResponse>(
        `/api/fridges/${fridgeId}/ingredients`,
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
