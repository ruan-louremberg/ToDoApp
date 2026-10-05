import { useState } from "react";
import { ApiError, changeTaskStatus } from "../api/tasks";
import type { TaskStatusDto } from "../types/task";

export function useChangeTaskStatus() {
  const [updatingTaskId, setUpdatingTaskId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function updateStatus(
    id: string,
    status: TaskStatusDto
  ): Promise<boolean> {
    setUpdatingTaskId(id);
    setError(null);

    try {
      await changeTaskStatus(id, { status });
      return true;
    } catch (err) {
      setError(
        err instanceof ApiError
          ? err.message
          : "Não foi possível alterar o status da tarefa."
      );
      return false;
    } finally {
      setUpdatingTaskId(null);
    }
  }

  return { updateStatus, updatingTaskId, error };
}