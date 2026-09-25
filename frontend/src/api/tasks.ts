const API_URL = import.meta.env.VITE_API_URL;
import type { ListTasksResponse } from "../types/ListTasksResponse";
import type { ProblemDetails } from "../types/problemDetails";
import type { CreateTaskData } from "../types/task";
import type { TaskFilters } from "../types/taskFilters";


export async function getTasks(filters?: TaskFilters): Promise<ListTasksResponse> {
    const params = new URLSearchParams();
    
    
    if (filters?.search) params.append("search", filters.search);

    if (filters?.status) params.append("status", filters.status);

    if (filters?.priority) params.append("priority", filters.priority);

    const queryString = params.toString() ? `?${params.toString()}` : "";

    const response = await fetch(`${API_URL}/api/tasks${queryString}`)

    if (!response.ok) {
        throw new Error("Erro ao buscar tarefas")
    }
    return response.json();
}


export async function createTask(
  data: CreateTaskData
): Promise<{ success: boolean; error?: string }> {
  try {
    const response = await fetch(`${API_URL}/api/tasks`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const problem: ProblemDetails = await response.json().catch(() => ({}));
      
      const errorMessage = problem.detail;
      
      return { success: false, error: errorMessage };
    }

    return { success: true };
  } catch {

    return { success: false, error: "Não foi possível conectar ao servidor." };
  }
}