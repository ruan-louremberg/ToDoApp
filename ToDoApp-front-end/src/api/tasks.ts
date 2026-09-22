const API_URL = import.meta.env.VITE_API_URL;
import type { ListTasksResponse } from "../types/ListTasksResponse";
import type { TaskFilters } from "../types/taskFilters";


export async function getTasks(filters?: TaskFilters): Promise<ListTasksResponse> {
    const params = new URLSearchParams();
    
    
    if (filters?.search) params.append("search", filters.search);

    if (filters?.status) params.append("status", filters.status);

    if (filters?.priority) params.append("priority", filters.priority);

    const queryString = params.toString() ? `?${params.toString()}` : "";

    const response = await fetch(`${API_URL}/api/task${queryString}`)

    if (!response.ok) {
        throw new Error("Erro ao buscar tarefas")
    }
    return response.json();
}