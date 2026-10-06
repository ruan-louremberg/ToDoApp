import { useState } from "react";
import { ApiError, restoreTask } from "../api/tasks";

export function useRestoreTask() {
  const [restoringTaskId, setRestoringTaskId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const restore = async (id: string): Promise<boolean> => {
    setRestoringTaskId(id);
    setError(null);

    try {
      await restoreTask(id);
      return true;
    } catch (err) {
      console.error(err);
      setError(
        err instanceof ApiError
          ? err.message
          : "Não foi possível restaurar a tarefa."
      );
      return false;
    } finally {
      setRestoringTaskId(null);
    }
  };

  return { restoringTaskId, error, restore };
}