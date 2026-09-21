const API_URL = import.meta.env.VITE_API_URL;
import type { ListTasksResponse } from "../types/ListTasksResponse";


export async function getTasks(): Promise<ListTasksResponse> {
    const response = await fetch(`${API_URL}/api/task`)
   
    if (!response.ok) {
        throw new Error("Erro ao buscar tarefas")
    }
    return response.json();
}