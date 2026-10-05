import { useState, useEffect, useCallback } from "react";
import { ApiError, getTasks } from "../api/tasks";
import type { ListTasksResponse } from "../types/ListTasksResponse";
import type { TaskFilters } from "../types/taskFilters";

export function useTasks(filters?: TaskFilters) {

  const [data, setData] = useState<ListTasksResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTasks = useCallback(async (isBackground = false) => {
    try {
      if (!isBackground) setLoading(true);
      setError(null);
      const result = await getTasks(filters);
      setData(result);

    } catch (err) {
      console.error(err);
      setError(
        err instanceof ApiError
          ? err.message
          : "Não foi possível carregar as tarefas."
      );

    } finally {
      if (!isBackground) setLoading(false);
    }
  }, [
    filters?.search,
    filters?.status,
    filters?.priority,
    filters?.page,
    filters?.pageSize,
  ]);

  useEffect(() => {
    fetchTasks(false);
    const intervalId = setInterval(() => {
      fetchTasks(true);
    }, 30000);

    const handleFocus = () => {
      fetchTasks(true);
    };

    window.addEventListener("focus", handleFocus);
    return () => {
      clearInterval(intervalId);
      window.removeEventListener("focus", handleFocus);
    };
  }, [fetchTasks]);

  return {
    data,
    loading,
    error,
    reload: () => fetchTasks(false),
  };
}