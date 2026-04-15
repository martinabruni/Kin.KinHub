import { useMutation, useQueryClient } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";
import { recipeBooksQueryKey } from "./useGetRecipeBooks";

export function useDeleteRecipeBook() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id: string) =>
      coreClient.delete<void>(`/api/recipe-books/${id}`, true),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: recipeBooksQueryKey });
    },
  });
}
