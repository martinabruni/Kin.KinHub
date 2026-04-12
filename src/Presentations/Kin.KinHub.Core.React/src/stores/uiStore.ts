import { create } from 'zustand'
import { persist } from 'zustand/middleware'

type Theme = 'light' | 'dark'
type Language = 'en' | 'it'

interface UiState {
  theme: Theme
  language: Language
  toggleTheme: () => void
  setLanguage: (lang: Language) => void
}

function applyTheme(theme: Theme) {
  if (theme === 'dark') {
    document.documentElement.classList.add('dark')
  } else {
    document.documentElement.classList.remove('dark')
  }
}

export const useUiStore = create<UiState>()(
  persist(
    (set, get) => ({
      theme: 'light',
      language: 'en',
      toggleTheme: () => {
        const next: Theme = get().theme === 'light' ? 'dark' : 'light'
        applyTheme(next)
        set({ theme: next })
      },
      setLanguage: (language) => set({ language }),
    }),
    {
      name: 'kinhub-ui',
      onRehydrateStorage: () => (state) => {
        if (state) applyTheme(state.theme)
      },
    },
  ),
)
