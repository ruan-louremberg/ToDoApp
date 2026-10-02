import { useState } from "react";
import { ApiError, editTask } from "../api/tasks";
import type { TaskFormData } from "../types/taskForm";

export function useEditTask() {
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function updateTask(id: string, data: TaskFormData): Promise<boolean> {
    setSaving(true);
    setError(null);

    try {
      await editTask(id, {
        title: data.title,
        description: data.description,
        priority: data.priority,
        dueDate: data.dueDate || null,
        categoryId: data.categoryId || null,
      });

      return true;
    } catch (err) {
      setError(
        err instanceof ApiError
          ? err.message
          : "Não foi possível editar a tarefa."
      );

      return false;
    } finally {
      setSaving(false);
    }
  }

  return {
    updateTask,
    saving,
    error,
    clearError: () => setError(null),
  };
}