import type { RefreshRequest, LoginResponse } from "@/types/identity";

export class ApiError extends Error {
  readonly status: number;
  readonly errors?: string[];

  constructor(status: number, message: string, errors?: string[]) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.errors = errors;
  }
}

// Lazy imports to avoid circular dependencies at module init time.
// authStore and router are accessed via getters passed by the app.
let getAccessToken: () => string | null = () => null;
let getRefreshToken: () => string | null = () => null;
let onTokensRefreshed: (r: LoginResponse) => void = () => {};
let onAuthFailure: () => void = () => {};

export function configureHttpClient(opts: {
  getAccessToken: () => string | null;
  getRefreshToken: () => string | null;
  onTokensRefreshed: (r: LoginResponse) => void;
  onAuthFailure: () => void;
}) {
  getAccessToken = opts.getAccessToken;
  getRefreshToken = opts.getRefreshToken;
  onTokensRefreshed = opts.onTokensRefreshed;
  onAuthFailure = opts.onAuthFailure;
}

async function parseError(res: Response): Promise<ApiError> {
  try {
    const body = (await res.json()) as { message?: string; errors?: string[] };
    return new ApiError(
      res.status,
      body.message ?? res.statusText,
      body.errors,
    );
  } catch {
    return new ApiError(res.status, res.statusText);
  }
}

export class HttpClient {
  private refreshPromise: Promise<LoginResponse> | null = null;
  private readonly baseUrl: string;

  constructor(baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  private async doRefresh(): Promise<LoginResponse> {
    const refreshToken = getRefreshToken();
    if (!refreshToken) throw new ApiError(401, "No refresh token available");

    const body: RefreshRequest = { refreshToken };
    const res = await fetch(`${this.baseUrl}/auth/refresh`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
      },
      body: JSON.stringify(body),
    });

    if (!res.ok) throw await parseError(res);
    return res.json() as Promise<LoginResponse>;
  }

  private async withRefreshRetry<T>(
    factory: (token: string | null) => Promise<Response>,
    requiresAuth: boolean,
  ): Promise<T> {
    let res = await factory(requiresAuth ? getAccessToken() : null);

    if (res.status === 401 && requiresAuth) {
      try {
        // Deduplicate concurrent refresh calls
        if (!this.refreshPromise) {
          this.refreshPromise = this.doRefresh().finally(() => {
            this.refreshPromise = null;
          });
        }
        const refreshed = await this.refreshPromise;
        onTokensRefreshed(refreshed);
        res = await factory(refreshed.accessToken);
      } catch {
        onAuthFailure();
        throw new ApiError(401, "Session expired");
      }
    }

    if (!res.ok) throw await parseError(res);
    if (res.status === 204) return undefined as T;
    return res.json() as Promise<T>;
  }

  async get<T>(path: string, requiresAuth = false): Promise<T> {
    return this.withRefreshRetry<T>(
      (token) =>
        fetch(`${this.baseUrl}${path}`, {
          method: "GET",
          headers: {
            Accept: "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
        }),
      requiresAuth,
    );
  }

  async post<T>(
    path: string,
    body?: unknown,
    requiresAuth = false,
  ): Promise<T> {
    return this.withRefreshRetry<T>(
      (token) =>
        fetch(`${this.baseUrl}${path}`, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: body !== undefined ? JSON.stringify(body) : undefined,
        }),
      requiresAuth,
    );
  }

  async patch<T>(
    path: string,
    body?: unknown,
    requiresAuth = false,
  ): Promise<T> {
    return this.withRefreshRetry<T>(
      (token) =>
        fetch(`${this.baseUrl}${path}`, {
          method: "PATCH",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: body !== undefined ? JSON.stringify(body) : undefined,
        }),
      requiresAuth,
    );
  }

  async put<T>(path: string, body?: unknown, requiresAuth = false): Promise<T> {
    return this.withRefreshRetry<T>(
      (token) =>
        fetch(`${this.baseUrl}${path}`, {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
          body: body !== undefined ? JSON.stringify(body) : undefined,
        }),
      requiresAuth,
    );
  }

  async delete<T>(path: string, requiresAuth = false): Promise<T> {
    return this.withRefreshRetry<T>(
      (token) =>
        fetch(`${this.baseUrl}${path}`, {
          method: "DELETE",
          headers: {
            Accept: "application/json",
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
          },
        }),
      requiresAuth,
    );
  }
}
