import { create } from "zustand";
import { persist, createJSONStorage } from "zustand/middleware";

export interface SelectedProfile {
  id: string;
  name: string;
  role: "admin" | "member";
  familyId: string;
}

interface ProfileState {
  selectedProfile: SelectedProfile | null;
  selectProfile: (profile: SelectedProfile) => void;
  clearProfile: () => void;
}

export const useProfileStore = create<ProfileState>()(
  persist(
    (set) => ({
      selectedProfile: null,
      selectProfile: (profile) => set({ selectedProfile: profile }),
      clearProfile: () => set({ selectedProfile: null }),
    }),
    {
      name: "kinhub-profile",
      storage: createJSONStorage(() => sessionStorage),
    },
  ),
);
