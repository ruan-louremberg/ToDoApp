import { useState } from "react";
import { createTask } from "../api/tasks";
import { TaskFormModal } from "../components/TaskFormModal";
import { useEditTask } from "../hooks/useEditTask";
import { useCategories } from "../hooks/useCategories";
import { useTasks } from "../hooks/useTasks";
import { useDebounce } from "../hooks/useDebounce";
import { useDeleteTask } from "../hooks/useDeleteTask";
import { useChangeTaskStatus } from "../hooks/useChangeTaskStatus";
import { TaskListLoadingState } from "../components/taskListStates";
import { TaskListErrorState } from "../components/taskListStates";
import { TaskListEmptyState } from "../components/taskListStates";
import { ConfirmDeleteModal } from "../components/ConfirmDeleteModal";
import { TaskFilters } from "../components/TaskFilters";
import { TaskRow } from "../components/TaskRow";
import type { TaskPriorityDto, TaskResponseList, TaskStatusDto } from "../types/task";
import type { TaskFormData } from "../types/taskForm";

export function TaskListPage() {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 500);
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [selectedTask, setSelectedTask] = useState<TaskResponseList | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [taskToDelete, setTaskToDelete] = useState<TaskResponseList | null>(null);
  const [createError, setCreateError] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);
  const statusFilter = status === "" ? undefined : Number(status) as TaskStatusDto;
  const priorityFilter = priority === "" ? undefined : Number(priority) as TaskPriorityDto;
  const { updateTask, saving: editing, error: editError } = useEditTask();
  const { categories, loading: categoriesLoading } = useCategories();
  const { data, loading, error, reload } = useTasks({
    search: debouncedSearch,
    status: statusFilter,
    priority: priorityFilter,
  });

  function openCreateModal() {
    setSelectedTask(null);
    setCreateError(null);
    setIsModalOpen(true);
  }

  function openEditModal(task: TaskResponseList) {
    setSelectedTask(task);
    setCreateError(null);
    setIsModalOpen(true);
  }

  async function handleSubmit(formData: TaskFormData) {
    if (selectedTask) {
      const updated = await updateTask(selectedTask.id, formData);

      if (updated) {
        await reload();
        setIsModalOpen(false);
      }

      return;
    }

    setCreating(true);
    setCreateError(null);

    try {
      await createTask({
        title: formData.title,
        description: formData.description,
        priority: formData.priority,
        dueDate: formData.dueDate || null,
        categoryId: formData.categoryId || null,
      });
      await reload();
      setIsModalOpen(false);
    } catch (err) {
      setCreateError(
        err instanceof Error
          ? err.message
          : "Não foi possível criar a tarefa."
      );
    } finally {
      setCreating(false);
    }
  }

  const formInitialData = selectedTask
    ? {
      title: selectedTask.title,
      description: selectedTask.description ?? "",
      priority: selectedTask.priority ?? 0,
      dueDate: selectedTask.dueDate?.slice(0, 10) ?? "",
      categoryId: selectedTask.category?.id ?? "",
    }
    : undefined;

  const {
    removeTask,
    deletingTaskId,
    error: deleteError,
  } = useDeleteTask();
  const {
    updateStatus,
    updatingTaskId,
    error: statusError,
  } = useChangeTaskStatus();

  async function handleTaskStatusChange(
    taskId: string,
    nextStatus: TaskStatusDto
  ): Promise<boolean> {
    const updated = await updateStatus(taskId, nextStatus);

    if (updated) {
      await reload();
    }

    return updated;
  }

  async function handleDeleteTask() {
    if (!taskToDelete) return;

    const deleted = await removeTask(taskToDelete.id);

    if (deleted) {
      await reload();
    }

    setTaskToDelete(null);
  }

  return (
    <main>
      <div className="task-toolbar">
        <button onClick={openCreateModal}>+ Nova Tarefa</button>

        <TaskFilters
          search={search}
          status={status}
          priority={priority}
          onSearchChange={setSearch}
          onStatusChange={setStatus}
          onPriorityChange={setPriority}
        />
      </div>
      {loading ? (
        <TaskListLoadingState />
      ) : error ? (
        <TaskListErrorState message={error} />
      ) : data?.items.length === 0 ? (
        <TaskListEmptyState />
      ) : (
        <div className="task-table-wrapper">
          <table className="task-table">
            <thead>
              <tr>
                <th scope="col">Título</th>
                <th scope="col">Descrição</th>
                <th scope="col">Prioridade</th>
                <th scope="col">Status</th>
                <th scope="col">Prazo</th>
                <th scope="col">Categoria</th>
                <th scope="col">Ações</th>
              </tr>
            </thead>
            <tbody>
              {data?.items.map((task) => (
                <TaskRow
                  key={task.id}
                  task={task}
                  isDeleteDisabled={
                    deletingTaskId !== null || updatingTaskId !== null
                  }
                  isDeleting={deletingTaskId === task.id}
                  isStatusDisabled={
                    updatingTaskId !== null || deletingTaskId !== null
                  }
                  isStatusUpdating={updatingTaskId === task.id}
                  onEdit={() => openEditModal(task)}
                  onDelete={() => setTaskToDelete(task)}
                  onChangeStatus={handleTaskStatusChange}
                />
              ))}
            </tbody>
          </table>
        </div>
      )}
      {deleteError && <p role="alert">{deleteError}</p>}
      {statusError && <p role="alert">{statusError}</p>}
      <ConfirmDeleteModal
        isOpen={taskToDelete !== null}
        taskTitle={taskToDelete?.title ?? ""}
        isLoading={deletingTaskId !== null}
        onClose={() => setTaskToDelete(null)}
        onConfirm={handleDeleteTask}
      />
      <TaskFormModal
        isOpen={isModalOpen}
        title={selectedTask ? "Editar tarefa" : "Nova tarefa"}
        submitLabel={selectedTask ? "Salvar alterações" : "Criar tarefa"}
        initialData={formInitialData}
        saving={selectedTask ? editing : creating}
        error={selectedTask ? editError : createError}
        categories={categories}
        categoriesLoading={categoriesLoading}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleSubmit}
      />
    </main>
  );
}