import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { fridgeIngredientsQueryKey } from "./useGetFridgeIngredients";

export function useDeleteFridgeIngredient(fridgeId: string) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (ingredientId: string) =>
      coreClient.delete<void>(
        `/api/fridges/${fridgeId}/ingredients/${ingredientId}`,
        true,
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: fridgeIngredientsQueryKey(fridgeId),
      });
    },
  });
}
