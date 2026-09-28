import { useEffect, useState } from "react";
import { getTaskSummary } from "../api/tasks";
import type { TaskSummaryResponse } from "../types/TaskSummaryResponse";

export function useTaskSummary() {
  const [data, setData] = useState<TaskSummaryResponse | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function fetchSummary() {
      try {
        setLoading(true);
        const result = await getTaskSummary();
        setData(result);
      } catch {
        setError("Erro ao carregar resumo");
      } finally {
        setLoading(false);
      }
    }

    fetchSummary();
  }, []);

  return { data, loading, error };
}