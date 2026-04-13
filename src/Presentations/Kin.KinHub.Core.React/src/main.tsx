import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import "./i18n";
import "./index.css";

import { router } from "./router";
import { configureHttpClient } from "./lib/http/httpClient";
import { useAuthStore } from "./stores/authStore";

// Wire up auth-aware HTTP client
configureHttpClient({
  getAccessToken: () => useAuthStore.getState().accessToken,
  getRefreshToken: () => useAuthStore.getState().refreshToken,
  onTokensRefreshed: (r) => useAuthStore.getState().login(r),
  onAuthFailure: () => {
    useAuthStore.getState().logout();
    router.navigate("/login", { replace: true });
  },
});

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60_000,
      retry: 2,
    },
    mutations: {
      retry: 2,
    },
  },
});

const root = document.getElementById("root")!;

createRoot(root).render(
  <StrictMode>
    <QueryClientProvider client={queryClient}>
      <RouterProvider router={router} />
    </QueryClientProvider>
  </StrictMode>,
);
