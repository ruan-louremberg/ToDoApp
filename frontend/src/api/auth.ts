import { apiFetch, throwApiError } from "./client";

const API_URL = import.meta.env.VITE_API_URL;

export interface AuthUser {
  id: string;
  name: string;
  email: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest extends LoginRequest {
  name: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  password: string;
}

export async function getCurrentUser(): Promise<AuthUser> {
  const response = await apiFetch(`${API_URL}/api/auth/me`);
  if (!response.ok) {
    await throwApiError(response, "Não foi possível verificar sua sessão.");
  }

  return response.json();
}

export async function loginUser(request: LoginRequest): Promise<AuthUser> {
  const response = await apiFetch(`${API_URL}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    await throwApiError(response, "Não foi possível entrar.");
  }

  return response.json();
}

export async function registerUser(request: RegisterRequest): Promise<AuthUser> {
  const response = await apiFetch(`${API_URL}/api/auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    await throwApiError(response, "Não foi possível criar sua conta.");
  }

  return response.json();
}

export async function requestPasswordReset(
  request: ForgotPasswordRequest
): Promise<{ message: string }> {
  const response = await apiFetch(`${API_URL}/api/auth/forgot-password`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    await throwApiError(response, "Não foi possível solicitar a redefinição.");
  }

  return response.json();
}

export async function resetPassword(
  request: ResetPasswordRequest
): Promise<void> {
  const response = await apiFetch(`${API_URL}/api/auth/reset-password`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    await throwApiError(response, "Não foi possível redefinir sua senha.");
  }
}

export async function logoutUser(): Promise<void> {
  const response = await apiFetch(`${API_URL}/api/auth/logout`, {
    method: "POST",
  });

  if (!response.ok) {
    await throwApiError(response, "Não foi possível encerrar sua sessão.");
  }
}
