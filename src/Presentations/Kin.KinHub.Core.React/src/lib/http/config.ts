export const IDENTITY_BASE_URL =
  (import.meta.env.VITE_IDENTITY_URL as string | undefined) ||
  "http://localhost:7000";

export const CORE_BASE_URL =
  (import.meta.env.VITE_CORE_URL as string | undefined) ||
  "http://localhost:7071";
