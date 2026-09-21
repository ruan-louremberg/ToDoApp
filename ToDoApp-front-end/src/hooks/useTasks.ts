import { useState, useEffect, useCallback } from "react";
import { getTasks } from "../api/tasks";
import type { ListTasksResponse } from "../types/ListTasksResponse";

export function useTasks() {
    const [ data, setData ] = useState<ListTasksResponse | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const fetchTasks = useCallback(async () => {
        try {
            setLoading(true);
            setError(null);
            const result = await getTasks();
            setData(result);
        }
        catch (err) {
            console.error(err)
            setError("erro ao carregar as tarefas");
        }
        finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        fetchTasks();
    },  [fetchTasks]);
    
    return {
        data,
        loading,
        error,
        reload: fetchTasks,
    };
}