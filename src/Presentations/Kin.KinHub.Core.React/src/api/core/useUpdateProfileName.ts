import { useMutation } from "@tanstack/react-query";
import { coreClient } from "@/lib/http/coreClient";

export function useUpdateProfileName() {
  return useMutation({
    mutationFn: ({ profileId, name }: { profileId: string; name: string }) =>
      coreClient.patch(`/api/profiles/${profileId}/name`, { name }, true),
  });
}
