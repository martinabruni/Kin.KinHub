---
name: react
description: Scaffolda o estende una React app feature-based per un backend developer. Usa quando devi creare componenti, provider, HTTP client, o nuove feature partendo da un OpenAPI spec.
---

# React App — Backend‑Developer‑Friendly Scaffold

## Stack obbligatorio

| Ruolo          | Libreria                                       |
| -------------- | ---------------------------------------------- |
| Bundler        | Vite + React + TypeScript (strict)             |
| UI Components  | ShadCN/ui (Radix UI primitives)                |
| Server state   | TanStack Query v5                              |
| Routing        | React Router v6                                |
| HTTP client    | Axios                                          |
| Validazione    | Zod                                            |
| Forms          | React Hook Form + Zod resolver                 |
| i18n           | i18next + react-i18next                        |
| Autenticazione | react-oidc-context (wrapper di oidc-client-ts) |
| Client state   | Zustand (solo se strettamente necessario)      |
| Linting        | ESLint + Prettier                              |

---

## Setup MCP ShadCN (obbligatorio — fai questo prima di tutto)

Per poter installare componenti ShadCN tramite linguaggio naturale, configura il MCP server aggiungendo il file `.vscode/mcp.json` alla radice del progetto:

```json
{
  "servers": {
    "shadcn": {
      "command": "npx",
      "args": ["shadcn@latest", "mcp"]
    }
  }
}
```

Dopo aver creato il file, aprilo in VS Code e clicca **Start** accanto al server `shadcn`.

Per inizializzare ShadCN nel progetto (se non già fatto):

```bash
npx shadcn@latest init
```

Da quel momento in poi, per aggiungere componenti usa linguaggio naturale oppure:

```bash
npx shadcn@latest add <component>
```

Tutti i componenti ShadCN vengono installati in `src/components/ui/` e importati da `@/components/ui/`. Non copiare mai manualmente il codice dei componenti ShadCN.

---

## Vincoli assoluti

- **Zero classi**: solo `type` e `interface` TypeScript.
- **Zero enum**: usa `as const` + `keyof typeof`.
- **Zero codice duplicato**: logica condivisa in `src/shared/` o `src/lib/`.
- **Tocca un solo file** per cambiare una cosa (testo, URL, colore, label).
- **TypeScript strict**: `"strict": true`, nessun `any` implicito.
- Tutti i testi in chiaro vanno nei file i18n (`src/i18n/locales/`), mai hardcoded nei componenti.
- Tutti i valori di config (base URL, timeout, feature flags, OIDC authority) vengono da `import.meta.env` → `src/config.ts`.

---

## Struttura cartelle

Adotta la struttura **Level 3 — Feature/Module based**:

```
src/
├── assets/
├── config.ts                   # unico punto per import.meta.env
├── i18n/
│   ├── index.ts
│   └── locales/
│       ├── it.json
│       └── en.json
├── lib/
│   ├── axios.ts                # httpClient con interceptor auth
│   └── queryClient.ts
├── shared/
│   ├── components/
│   │   ├── ErrorBoundary.tsx
│   │   └── ProtectedRoute.tsx  # guard per route autenticate
│   ├── hooks/
│   │   └── useAuth.ts          # wrapper su react-oidc-context
│   └── types/
│       └── api.types.ts
├── features/
│   └── {feature}/
│       ├── api/
│       │   └── {feature}.api.ts
│       ├── components/
│       ├── hooks/
│       │   └── use{Feature}.ts
│       ├── types/
│       │   └── {feature}.types.ts
│       └── index.ts
├── pages/
│   ├── {Feature}Page.tsx
│   └── auth/
│       ├── CallbackPage.tsx    # redirect URI dopo login OIDC
│       └── SilentRenewPage.tsx
├── router/
│   └── index.tsx
├── providers/
│   └── AppProviders.tsx        # OidcProvider + QueryClientProvider + Router
└── main.tsx
```

---

## Pattern obbligatori da implementare

### 1 — `src/config.ts` — unico punto di configurazione

```typescript
const config = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL,
  defaultLocale: import.meta.env.VITE_DEFAULT_LOCALE ?? "it",
  requestTimeoutMs: Number(import.meta.env.VITE_REQUEST_TIMEOUT_MS ?? 10000),
  oidc: {
    authority: import.meta.env.VITE_OIDC_AUTHORITY,
    clientId: import.meta.env.VITE_OIDC_CLIENT_ID,
    redirectUri: import.meta.env.VITE_OIDC_REDIRECT_URI,
    postLogoutRedirectUri: import.meta.env.VITE_OIDC_POST_LOGOUT_REDIRECT_URI,
    scope: import.meta.env.VITE_OIDC_SCOPE ?? "openid profile email",
  },
} as const;

export type AppConfig = typeof config;
export default config;
```

### 2 — `src/providers/AppProviders.tsx` — OidcProvider come root

```typescript
import { AuthProvider } from 'react-oidc-context';
import { QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import { BrowserRouter } from 'react-router-dom';
import { queryClient } from '@/lib/queryClient';
import config from '@/config';
import '@/i18n';

const oidcConfig = {
  authority: config.oidc.authority,
  client_id: config.oidc.clientId,
  redirect_uri: config.oidc.redirectUri,
  post_logout_redirect_uri: config.oidc.postLogoutRedirectUri,
  scope: config.oidc.scope,
  automaticSilentRenew: true,
};

export const AppProviders = ({ children }: { children: React.ReactNode }) => (
  <AuthProvider {...oidcConfig}>
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
      <ReactQueryDevtools initialIsOpen={false} />
    </QueryClientProvider>
  </AuthProvider>
);
```

### 3 — `src/shared/hooks/useAuth.ts` — wrapper tipizzato su react-oidc-context

```typescript
import { useAuth as useOidcAuth } from "react-oidc-context";

export const useAuth = () => {
  const auth = useOidcAuth();

  return {
    isAuthenticated: auth.isAuthenticated,
    isLoading: auth.isLoading,
    user: auth.user,
    accessToken: auth.user?.access_token ?? null,
    login: () => auth.signinRedirect(),
    logout: () => auth.signoutRedirect(),
    error: auth.error ?? null,
  };
};
```

Regola: nei componenti e negli hook delle feature, importa sempre `useAuth` da `@/shared/hooks/useAuth`, mai direttamente da `react-oidc-context`.

### 4 — `src/lib/axios.ts` — HTTP client con token JWT iniettato automaticamente

```typescript
import axios, { AxiosError } from "axios";
import { User } from "oidc-client-ts";
import config from "@/config";
import type { ApiError } from "@/shared/types/api.types";

export const httpClient = axios.create({
  baseURL: config.apiBaseUrl,
  timeout: config.requestTimeoutMs,
  headers: { "Content-Type": "application/json" },
});

// Legge il token direttamente dallo storage di oidc-client-ts (memory-safe)
const getAccessToken = (): string | null => {
  const key = `oidc.user:${config.oidc.authority}:${config.oidc.clientId}`;
  const raw = sessionStorage.getItem(key);
  if (!raw) return null;
  const user = User.fromStorageString(raw);
  return user.access_token ?? null;
};

httpClient.interceptors.request.use((req) => {
  const token = getAccessToken();
  if (token) req.headers.Authorization = `Bearer ${token}`;
  return req;
});

httpClient.interceptors.response.use(
  (res) => res,
  (error: AxiosError) => {
    const apiError: ApiError = {
      status: error.response?.status ?? 0,
      message:
        (error.response?.data as { message?: string })?.message ??
        error.message,
      detail: error.response?.data,
    };
    return Promise.reject(apiError);
  },
);
```

> **Nota sicurezza**: `oidc-client-ts` usa `sessionStorage` di default per i token, non `localStorage`. Non spostare i token in `localStorage` — è vulnerabile a XSS. Se l'Authorization Server supporta i cookie HttpOnly, preferisci quello.

### 5 — `src/shared/components/ProtectedRoute.tsx` — guard per route autenticate

```typescript
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '@/shared/hooks/useAuth';

export const ProtectedRoute = () => {
  const { isAuthenticated, isLoading, login } = useAuth();
  const location = useLocation();

  if (isLoading) return <div>...</div>;

  if (!isAuthenticated) {
    login();
    return <Navigate to="/" state={{ from: location }} replace />;
  }

  return <Outlet />;
};
```

### 6 — `src/pages/auth/CallbackPage.tsx` — gestione redirect URI

```typescript
import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth as useOidcAuth } from 'react-oidc-context';

export const CallbackPage = () => {
  const auth = useOidcAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!auth.isLoading && auth.isAuthenticated) {
      navigate('/', { replace: true });
    }
  }, [auth.isLoading, auth.isAuthenticated, navigate]);

  if (auth.error) return <div>Errore autenticazione: {auth.error.message}</div>;

  return <div>Accesso in corso...</div>;
};
```

### 7 — `src/router/index.tsx` — route protette e callback OIDC

```typescript
import { Routes, Route } from 'react-router-dom';
import { lazy, Suspense } from 'react';
import { ProtectedRoute } from '@/shared/components/ProtectedRoute';
import { CallbackPage } from '@/pages/auth/CallbackPage';

const {Feature}Page = lazy(() => import('@/pages/{Feature}Page'));

export const AppRouter = () => (
  <Suspense fallback={<div>...</div>}>
    <Routes>
      {/* Callback OIDC — deve essere pubblica */}
      <Route path="/auth/callback" element={<CallbackPage />} />

      {/* Route protette */}
      <Route element={<ProtectedRoute />}>
        <Route path="/{feature}s" element={<{Feature}Page />} />
      </Route>
    </Routes>
  </Suspense>
);
```

### 8 — `src/shared/types/api.types.ts`

```typescript
export interface ApiError {
  status: number;
  message: string;
  detail?: unknown;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
```

### 9 — feature API layer (1 funzione = 1 endpoint)

```typescript
// src/features/{feature}/api/{feature}.api.ts
import { httpClient } from '@/lib/axios';
import type { {Feature}, Create{Feature}Request } from '../types/{feature}.types';

const BASE = '/{feature}s';

export const {feature}Api = {
  getAll: () =>
    httpClient.get<{Feature}[]>(BASE).then((r) => r.data),

  getById: (id: string) =>
    httpClient.get<{Feature}>(`${BASE}/${id}`).then((r) => r.data),

  create: (body: Create{Feature}Request) =>
    httpClient.post<{Feature}>(BASE, body).then((r) => r.data),

  update: (id: string, body: Partial<Create{Feature}Request>) =>
    httpClient.put<{Feature}>(`${BASE}/${id}`, body).then((r) => r.data),

  remove: (id: string) =>
    httpClient.delete(`${BASE}/${id}`),
} as const;
```

### 10 — hook TanStack Query (= "provider" per la feature)

```typescript
// src/features/{feature}/hooks/use{Feature}.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { {feature}Api } from '../api/{feature}.api';

const QUERY_KEY = ['{feature}s'] as const;

export const use{Feature}List = () =>
  useQuery({ queryKey: QUERY_KEY, queryFn: {feature}Api.getAll });

export const use{Feature}Detail = (id: string) =>
  useQuery({ queryKey: [...QUERY_KEY, id], queryFn: () => {feature}Api.getById(id) });

export const useCreate{Feature} = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: {feature}Api.create,
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEY }),
  });
};

export const useDelete{Feature} = () => {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: {feature}Api.remove,
    onSuccess: () => qc.invalidateQueries({ queryKey: QUERY_KEY }),
  });
};
```

### 11 — i18n setup

```typescript
// src/i18n/index.ts
import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import it from "./locales/it.json";
import en from "./locales/en.json";
import config from "@/config";

i18n.use(initReactI18next).init({
  resources: { it: { translation: it }, en: { translation: en } },
  lng: config.defaultLocale,
  fallbackLng: "it",
  interpolation: { escapeValue: false },
});

export default i18n;
```

Struttura chiave i18n — piatta e feature-scoped:

```json
{
  "{feature}": {
    "title": "...",
    "list": { "empty": "Nessun elemento." },
    "form": { "name": "Nome", "submit": "Salva" },
    "errors": { "load": "Errore nel caricamento." }
  },
  "common": {
    "loading": "Caricamento...",
    "error": "Si è verificato un errore.",
    "confirm": "Conferma",
    "cancel": "Annulla"
  },
  "auth": {
    "login": "Accedi",
    "logout": "Esci",
    "error": "Errore durante l'autenticazione."
  }
}
```

---

## Quando fornisco un file `openapi.yml`

1. Leggi ogni `paths` entry e mappa ogni operazione → una funzione in `{feature}.api.ts`.
2. Genera i tipi TypeScript dai `components/schemas` → `{feature}.types.ts`. Usa `interface` per oggetti, `type` per union/alias.
3. Genera un hook TanStack Query per ogni operazione `GET` (useQuery) e per ogni `POST/PUT/PATCH/DELETE` (useMutation).
4. Se lo spec contiene `securitySchemes` di tipo `openIdConnect` o `oauth2`, usa quei valori per popolare le variabili OIDC in `.env.example`.
5. Aggiungi le chiavi i18n nei file di locale per ogni label, messaggio di errore e placeholder visibile.
6. Non generare codice fuori dalla struttura sopra descritta.

---

## Checklist prima del commit

- [ ] Nessuna stringa in chiaro nei componenti
- [ ] Nessun valore di config hardcoded (URL, timeout, chiavi, authority OIDC)
- [ ] Nessuna chiamata Axios fuori da `features/{feature}/api/`
- [ ] Token mai salvati in `localStorage`
- [ ] `useAuth` importato sempre da `@/shared/hooks/useAuth`
- [ ] Tutte le route applicative avvolte in `<ProtectedRoute />`
- [ ] La route `/auth/callback` è pubblica
- [ ] Nessun `any` esplicito o implicito
- [ ] Ogni nuova feature ha il suo `index.ts` barrel export
- [ ] `vite build` termina senza errori
- [ ] Tutti i testi tradotti in `it.json` e `en.json`

---

## `.env.example` da creare alla radice

```env
VITE_API_BASE_URL=http://localhost:5000
VITE_DEFAULT_LOCALE=it
VITE_REQUEST_TIMEOUT_MS=10000

# OIDC / OAuth2 — valori forniti dall'Authorization Server (es. Keycloak, Azure AD, Auth0)
VITE_OIDC_AUTHORITY=https://your-auth-server.com/realms/your-realm
VITE_OIDC_CLIENT_ID=your-client-id
VITE_OIDC_REDIRECT_URI=http://localhost:5173/auth/callback
VITE_OIDC_POST_LOGOUT_REDIRECT_URI=http://localhost:5173
VITE_OIDC_SCOPE=openid profile email
```
