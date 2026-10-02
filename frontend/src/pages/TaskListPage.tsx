import { useState } from "react";
import { createTask } from "../api/tasks";
import { TaskFormModal } from "../components/TaskFormModal";
import { useEditTask } from "../hooks/useEditTask";
import { useCategories } from "../hooks/useCategories";
import { useTasks } from "../hooks/useTasks";
import { useDebounce } from "../hooks/useDebounce";
import { useDeleteTask } from "../hooks/useDeleteTask";
import type { TaskPriorityDto, TaskResponseList, TaskStatusDto } from "../types/task";
import type { TaskFormData } from "../types/taskForm";

function getPriorityLabel(priority: TaskPriorityDto | null) {
  const labels: Record<string, string> = {
    "0": "Baixa",
    "1": "Média",
    "2": "Alta",
    Low: "Baixa",
    Medium: "Média",
    High: "Alta",
  };

  return labels[String(priority)] ?? String(priority);
}

function formatDueDate(dueDate: string | null) {
  if (!dueDate) return "Sem prazo";

  return new Intl.DateTimeFormat("pt-BR").format(
    new Date(`${dueDate.slice(0, 10)}T00:00:00`)
  );
}

export function TaskListPage() {
  const [search, setSearch] = useState("");
  const debouncedSearch = useDebounce(search, 500);
  const [status, setStatus] = useState("");
  const [priority, setPriority] = useState("");
  const [selectedTask, setSelectedTask] = useState<TaskResponseList | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
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

  async function handleDeleteTask(task: TaskResponseList) {
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
      <div className="task-toolbar">
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
          <option value="0">Pendente</option>
          <option value="1">Em Progresso</option>
          <option value="2">Concluída</option>
        </select>

        <select
          value={priority}
          onChange={(event) => setPriority(event.target.value)}
        >
          <option value="">Todas as Prioridades</option>
          <option value="0">Baixa</option>
          <option value="1">Média</option>
          <option value="2">Alta</option>
        </select>
      </div>
      </div>
      {loading ? (
        <p>Carregando tarefas...</p>
      ) : error ? (
        <p>{error}</p>
      ) : data?.items.length === 0 ? (
        <p>Nenhuma tarefa cadastrada ainda.</p>
      ) : (
        <div className="task-table-wrapper">
          <table className="task-table">
            <thead>
              <tr>
                <th scope="col">Prioridade</th>
                <th scope="col">Título</th>
                <th scope="col">Descrição</th>
                <th scope="col">Prazo</th>
                <th scope="col">Categoria</th>
                <th scope="col">Ações</th>
              </tr>
            </thead>
            <tbody>
              {data?.items.map((task) => (
                <tr key={task.id}>
                  <td>{getPriorityLabel(task.priority)}</td>
                  <td className="task-title-cell">{task.title}</td>
                  <td>
                    {task.description ? (
                      <details className="task-description">
                        <summary>Ver descrição</summary>
                        <p>{task.description}</p>
                      </details>
                    ) : (
                      <span className="muted-cell">Sem descrição</span>
                    )}
                  </td>
                  <td>{formatDueDate(task.dueDate)}</td>
                  <td>{task.category?.name ?? "Sem categoria"}</td>
                  <td className="task-actions">
                    <button type="button" onClick={() => openEditModal(task)}>
                      Editar
                    </button>
                    <button
                      type="button"
                      disabled={deletingTaskId !== null}
                      onClick={() => void handleDeleteTask(task)}
                    >
                      {deletingTaskId === task.id ? "Excluindo..." : "Excluir"}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
      {deleteError && <p role="alert">{deleteError}</p>}
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