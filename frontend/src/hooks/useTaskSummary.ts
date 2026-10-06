import { useCallback, useEffect, useState } from "react";
import { getTaskSummary } from "../api/tasks";
import type { TaskSummaryResponse } from "../types/TaskSummaryResponse";

interface UseTaskSummaryResult {
  summary: TaskSummaryResponse | null;
  isLoading: boolean;
  error: string | null;
  reload: () => Promise<void>;
}

export function useTaskSummary(): UseTaskSummaryResult {
  const [summary, setSummary] =
    useState<TaskSummaryResponse | null>(null);

  const [isLoading, setIsLoading] =
    useState(true);

  const [error, setError] =
    useState<string | null>(null);

  const fetchSummary = useCallback(async (isBackground = false) => {
    try {
      if (!isBackground) {
        setIsLoading(true);
      }

      setError(null);

      const data = await getTaskSummary();

      setSummary(data);
    } catch {
      setError(
        "Não foi possível atualizar o resumo das tarefas."
      );
    } finally {
      if (!isBackground) {
        setIsLoading(false);
      }
    }
  }, []);

  const reload = useCallback(() => fetchSummary(false), [fetchSummary]);

  useEffect(() => {
    fetchSummary(false);

    const intervalId = window.setInterval(() => {
      fetchSummary(true);
    }, 30_000);

    const handleFocus = () => {
      fetchSummary(true);
    };

    window.addEventListener("focus", handleFocus);

    return () => {
      window.clearInterval(intervalId);
      window.removeEventListener("focus", handleFocus);
    };
  }, [fetchSummary]);

  return {
    summary,
    isLoading,
    error,
    reload,
  };
}