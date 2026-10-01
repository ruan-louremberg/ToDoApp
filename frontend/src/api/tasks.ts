const API_URL = import.meta.env.VITE_API_URL;

import type { ListTasksResponse } from "../types/ListTasksResponse";
import type { ProblemDetails } from "../types/problemDetails";
import type { CompleteTaskRequest, CreateTaskRequest, UpdateTaskRequest } from "../types/task";
import type { ListTasksRequest } from "../types/taskFilters";
import type { TaskSummaryResponse } from "../types/TaskSummaryResponse";

export class ApiError extends Error {
  readonly problemDetails?: ProblemDetails;

  constructor(
    message: string,
    problemDetails?: ProblemDetails
  ) {
    super(message);
    this.problemDetails = problemDetails;
    this.name = "ApiError";
  }
}

async function throwApiError(response: Response, fallbackMessage: string): Promise<never> {
  const problem: ProblemDetails = await response.json().catch(() => ({}));

  throw new ApiError(
    problem.detail ?? fallbackMessage,
    problem
  );
}


export async function getTasks(filters?: ListTasksRequest): Promise<ListTasksResponse> {
    const params = new URLSearchParams();
    
    
    if (filters?.search) params.append("search", filters.search);

    if (filters?.status !== undefined) params.append("status", String(filters.status));

    if (filters?.priority !== undefined) params.append("priority", String(filters.priority));
    if (filters?.sortBy) params.append("sortBy", filters.sortBy);
    if (filters?.sortDirection) params.append("sortDirection", filters.sortDirection);
    if (filters?.categoryId) params.append("categoryId", filters.categoryId);
    if (filters?.page !== undefined) params.append("page", String(filters.page));
    if (filters?.pageSize !== undefined) params.append("pageSize", String(filters.pageSize));

    const queryString = params.toString() ? `?${params.toString()}` : "";

    const response = await fetch(`${API_URL}/api/tasks${queryString}`)

    if (!response.ok) {
        await throwApiError(response, "Erro ao buscar tarefas");
    }
    return response.json();
}


export async function createTask(
  data: CreateTaskRequest
): Promise<{ success: boolean; error?: string }> {
  const response = await fetch(`${API_URL}/api/tasks`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao criar tarefa");
  }

  return { success: true };
}

export async function editTask(
  id: string,
  data: UpdateTaskRequest
): Promise<{ success: boolean; error?: string }> {
  const response = await fetch(`${API_URL}/api/tasks/${id}`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao editar tarefa");
  }

  return { success: true };
}

export async function deleteTask(
  id: string
): Promise<{ success: boolean; error?: string }> {
  const response = await fetch(`${API_URL}/api/tasks/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao excluir tarefa");
  }

  return { success: true };
}


export async function completeTask(
  id: string,
  data: CompleteTaskRequest = { status: 2 }
): Promise<{ success: boolean; error?: string }> {
  const response = await fetch(`${API_URL}/api/tasks/${id}/status`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao concluir tarefa");
  }

  return { success: true };
}

export async function getTaskSummary(): Promise<TaskSummaryResponse> {
    const response = await fetch(`${API_URL}/api/tasks/summary`);
    if (!response.ok) {
        await throwApiError(response, "Erro ao buscar resumo das tarefas");
    }
    return response.json();
}