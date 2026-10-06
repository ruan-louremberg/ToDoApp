import { useCallback, useEffect, useState } from "react";
import { ApiError, getTrashTasks } from "../api/tasks";
import type { ListTasksResponse } from "../types/ListTasksResponse";

export function useTrashTasks(page: number, pageSize: number) {
  const [data, setData] = useState<ListTasksResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchTrashTasks = useCallback(async (isBackground = false) => {
    try {
      if (!isBackground) setLoading(true);
      setError(null);
      const result = await getTrashTasks(page, pageSize);
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
  }, [page, pageSize]);

  useEffect(() => {
    fetchTrashTasks();
  }, [fetchTrashTasks]);

  return { data, loading, error, fetchTrashTasks };
}