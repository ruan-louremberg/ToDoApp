import { useState } from "react";
import { createTask } from "../api/tasks";
import { TaskFormModal } from "../components/TaskFormModal";
import { useEditTask } from "../hooks/useEditTask";
import { useTasks } from "../hooks/useTasks";
import { useDebounce } from "../hooks/useDebounce";
import { useDeleteTask } from "../hooks/useDeleteTask";
import type { Task } from "../types/task";
import type { TaskFormData } from "../types/taskForm";

export function TaskListPage() {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 500);
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [selectedTask, setSelectedTask] = useState<Task | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [createError, setCreateError] = useState<string | null>(null);
  const [creating, setCreating] = useState(false);
  const { updateTask, saving: editing, error: editError } = useEditTask();
  const { data, loading, error, reload } = useTasks({
    search: debouncedSearch,
    status,
    priority,
  });

  function openCreateModal() {
    setSelectedTask(null);
    setCreateError(null);
    setIsModalOpen(true);
  }

  function openEditModal(task: Task) {
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
        priority: Number(selectedTask.priority),
        dueDate: selectedTask.dueDate?.slice(0, 10) ?? "",
      }
    : undefined;

  const {
    removeTask,
    deletingTaskId,
    error: deleteError,
  } = useDeleteTask();

  async function handleDeleteTask(task: Task) {
    const confirmed = window.confirm(
      `Tem certeza que deseja excluir "${task.title}"?`
    );

    if (!confirmed) return;

    const deleted = await removeTask(task.id);

    if (deleted) {
      await reload();
    }
  }

  return (
    <main>
      <h1>Minhas Tarefas</h1>
      <button onClick={openCreateModal}>+ Nova Tarefa</button>
      <div className="filters">
        <input
          type="text"
          placeholder="buscar por título..."
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />

        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          <option value="">Todos os Status</option>
          <option value="Pending">Pendente</option>
          <option value="InProgress">Em Progresso</option>
          <option value="Completed">Concluída</option>
        </select>

        <select
          value={priority}
          onChange={(event) => setPriority(event.target.value)}
        >
          <option value="">Todas as Prioridades</option>
          <option value="Low">Baixa</option>
          <option value="Medium">Média</option>
          <option value="High">Alta</option>
        </select>
      </div>
      {loading ? (
        <p>Carregando tarefas...</p>
      ) : error ? (
        <p>{error}</p>
      ) : data?.items.length === 0 ? (
        <p>Nenhuma tarefa cadastrada ainda.</p>
      ) : (
        <ul>
          {data?.items.map((task) => (
            <li key={task.id}>
              <strong>{task.title}</strong> - {task.description}
              <button type="button" onClick={() => openEditModal(task)}>
                Editar
              </button>
              <button
                type="button"
                disabled={deletingTaskId !== null}
                onClick={() => void handleDeleteTask(task)}>
                {deletingTaskId === task.id ? "Excluindo..." : "Excluir"}
              </button>
            </li>
          ))}
        </ul>
      )}
      {deleteError && <p role="alert">{deleteError}</p>}
      <TaskFormModal
        isOpen={isModalOpen}
        title={selectedTask ? "Editar tarefa" : "Nova tarefa"}
        submitLabel={selectedTask ? "Salvar alterações" : "Criar tarefa"}
        initialData={formInitialData}
        saving={selectedTask ? editing : creating}
        error={selectedTask ? editError : createError}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleSubmit}
      />
    </main>
  );
}
