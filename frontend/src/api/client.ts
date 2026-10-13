import type { ProblemDetails } from "../types/problemDetails";

export class ApiError extends Error {
  readonly problemDetails?: ProblemDetails;
  readonly status: number;

  constructor(
    message: string,
    status: number,
    problemDetails?: ProblemDetails
  ) {
    super(message);
    this.problemDetails = problemDetails;
    this.status = status;
    this.name = "ApiError";
  }
}

export function apiFetch(
  input: RequestInfo | URL,
  init: RequestInit = {}
): Promise<Response> {
  return fetch(input, {
    ...init,
    credentials: "include",
  });
}

export async function throwApiError(
  response: Response,
  fallbackMessage: string
): Promise<never> {
  const problem: ProblemDetails = await response.json().catch(() => ({}));

  throw new ApiError(
    problem.detail ?? problem.title ?? fallbackMessage,
    response.status,
    problem
  );
}
