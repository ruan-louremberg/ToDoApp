const API_URL = import.meta.env.VITE_API_URL;

import { apiFetch, throwApiError } from "./client";
export { ApiError } from "./client";

import type { ListTasksResponse } from "../types/ListTasksResponse";
import type {
  ChangeTaskStatusRequest,
  CreateTaskRequest,
  SubTask,
  UpdateTaskRequest,
} from "../types/task";
import type { ListTasksRequest } from "../types/taskFilters";
import type { TaskSummaryResponse } from "../types/TaskSummaryResponse";

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

    const response = await apiFetch(`${API_URL}/api/tasks${queryString}`)

    if (!response.ok) {
        await throwApiError(response, "Erro ao buscar tarefas");
    }
    return response.json();
}


export async function createTask(
  data: CreateTaskRequest
): Promise<{ success: boolean; error?: string }> {
  const response = await apiFetch(`${API_URL}/api/tasks`, {
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
  const response = await apiFetch(`${API_URL}/api/tasks/${id}`, {
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
  const response = await apiFetch(`${API_URL}/api/tasks/${id}`, {
    method: "DELETE",
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao excluir tarefa");
  }

  return { success: true };
}

export async function getTrashTasks(
  page = 1,
  pageSize = 20
): Promise<ListTasksResponse> {
  const params = new URLSearchParams({
    page: String(page),
    pageSize: String(pageSize),
  });
  const response = await apiFetch(`${API_URL}/api/tasks/trash?${params}`);

  if (!response.ok) {
    await throwApiError(response, "Erro ao buscar tarefas excluídas");
  }

  return response.json();
}

export async function restoreTask(
  id: string
): Promise<{ success: boolean; error?: string }> {
  const response = await apiFetch(`${API_URL}/api/tasks/${id}/restore`, {
    method: "PATCH",
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao restaurar tarefa");
  }

  return { success: true };
}

export async function changeTaskStatus(
  id: string,
  data: ChangeTaskStatusRequest
): Promise<{ success: boolean; error?: string }> {
  const response = await apiFetch(`${API_URL}/api/tasks/${id}/status`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    await throwApiError(response, "Erro ao alterar status da tarefa");
  }

  return { success: true };
}

export async function getTaskSummary(): Promise<TaskSummaryResponse> {
    const response = await apiFetch(`${API_URL}/api/tasks/summary`);
    if (!response.ok) {
        await throwApiError(response, "Erro ao buscar resumo das tarefas");
    }
    return response.json();
}

export async function getSubTasks(taskId: string): Promise<SubTask[]> {
  const response = await apiFetch(`${API_URL}/api/tasks/${taskId}/subtasks`);
  if (!response.ok) await throwApiError(response, "Erro ao buscar subtarefas");
  return response.json();
}

export async function createSubTask(taskId: string, title: string): Promise<SubTask> {
  const response = await apiFetch(`${API_URL}/api/tasks/${taskId}/subtasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ title }),
  });
  if (!response.ok) await throwApiError(response, "Erro ao criar subtarefa");
  return response.json();
}

export async function updateSubTask(
  taskId: string,
  subtaskId: string,
  data: { title?: string; isCompleted?: boolean }
): Promise<SubTask> {
  const response = await apiFetch(
    `${API_URL}/api/tasks/${taskId}/subtasks/${subtaskId}`,
    {
      method: "PATCH",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    }
  );
  if (!response.ok) await throwApiError(response, "Erro ao atualizar subtarefa");
  return response.json();
}

export async function deleteSubTask(taskId: string, subtaskId: string): Promise<void> {
  const response = await apiFetch(
    `${API_URL}/api/tasks/${taskId}/subtasks/${subtaskId}`,
    { method: "DELETE" }
  );
  if (!response.ok) await throwApiError(response, "Erro ao excluir subtarefa");
}

export async function reorderSubTasks(taskId: string, subtaskIds: string[]): Promise<void> {
  const response = await apiFetch(`${API_URL}/api/tasks/${taskId}/subtasks/order`, {
    method: "PATCH",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ subtaskIds }),
  });
  if (!response.ok) await throwApiError(response, "Erro ao reordenar subtarefas");
}