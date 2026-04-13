import { create } from "zustand";
import { persist } from "zustand/middleware";

type Theme = "light" | "dark";
type Language = "en" | "it";

interface SnackbarState {
  message: string;
  id: number;
}

interface UiState {
  theme: Theme;
  language: Language;
  snackbar: SnackbarState | null;
  toggleTheme: () => void;
  setLanguage: (lang: Language) => void;
  showSnackbar: (message: string) => void;
  hideSnackbar: () => void;
}

function applyTheme(theme: Theme) {
  if (theme === "dark") {
    document.documentElement.classList.add("dark");
  } else {
    document.documentElement.classList.remove("dark");
  }
}

export const useUiStore = create<UiState>()(
  persist(
    (set, get) => ({
      theme: "light",
      language: "en",
      snackbar: null,
      toggleTheme: () => {
        const next: Theme = get().theme === "light" ? "dark" : "light";
        applyTheme(next);
        set({ theme: next });
      },
      setLanguage: (language) => set({ language }),
      showSnackbar: (message) => set({ snackbar: { message, id: Date.now() } }),
      hideSnackbar: () => set({ snackbar: null }),
    }),
    {
      name: "kinhub-ui",
      onRehydrateStorage: () => (state) => {
        if (state) applyTheme(state.theme);
      },
    },
  ),
);
