import { useState } from "react";
import { ApiError, deleteTask } from "../api/tasks";

export function useDeleteTask() {
  const [deletingTaskId, setDeletingTaskId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function removeTask(id: string): Promise<boolean> {
    setDeletingTaskId(id);
    setError(null);

    try {
      await deleteTask(id);
      return true;
    } catch (err) {
      setError(
        err instanceof ApiError
          ? err.message
          : "Não foi possível excluir a tarefa."
      );
      return false;
    } finally {
      setDeletingTaskId(null);
    }
  }

  return { removeTask, deletingTaskId, error };
}