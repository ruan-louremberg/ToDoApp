import { useState, type FormEvent } from "react";
import { Check, ChevronDown, ChevronRight, GripVertical, Plus, Trash2 } from "lucide-react";
import {
  createSubTask,
  deleteSubTask,
  getSubTasks,
  reorderSubTasks,
  updateSubTask,
} from "../api/tasks";
import { TaskActionsMenu } from "./TaskActionsMenu";
import { TaskBadge } from "./TaskBadge";
import type { SubTask, TaskResponseList, TaskStatusDto } from "../types/task";

interface TaskRowProps {
  task: TaskResponseList;
  isDeleteDisabled: boolean;
  isDeleting: boolean;
  isStatusDisabled: boolean;
  isStatusUpdating: boolean;
  onEdit: () => void;
  onDelete: () => void;
  onChangeStatus: (
    taskId: string,
    nextStatus: TaskStatusDto
  ) => Promise<boolean>;
}
    
function formatDueDate(dueDate: string | null) {
  if (!dueDate) return "Sem prazo";

  return new Intl.DateTimeFormat("pt-BR").format(
    new Date(`${dueDate.slice(0, 10)}T00:00:00`)
  );
}

export function TaskRow({
  task,
  isDeleteDisabled,
  isDeleting,
  isStatusDisabled,
  isStatusUpdating,
  onEdit,
  onDelete,
  onChangeStatus,
}: TaskRowProps) {
  const [expanded, setExpanded] = useState(false);
  const [items, setItems] = useState<SubTask[] | null>(null);
  const [newTitle, setNewTitle] = useState("");
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [draggedId, setDraggedId] = useState<string | null>(null);

  async function run(action: () => Promise<void>, fallback: string) {
    setSaving(true);
    setError(null);
    try {
      await action();
    } catch (err) {
      setError(err instanceof Error ? err.message : fallback);
    } finally {
      setSaving(false);
    }
  }

  async function toggleChecklist() {
    if (expanded) {
      setExpanded(false);
      return;
    }
    setExpanded(true);
    if (items !== null) return;
    setLoading(true);
    try {
      setItems(await getSubTasks(task.id));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar subtarefas.");
    } finally {
      setLoading(false);
    }
  }

  async function addSubtask(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    await run(async () => {
      const created = await createSubTask(task.id, newTitle);
      setItems((current) => [...(current ?? []), created].sort((a, b) => a.position - b.position));
      setNewTitle("");
    }, "Erro ao criar subtarefa.");
  }

  function toggleSubtask(subtask: SubTask) {
    void run(async () => {
      const updated = await updateSubTask(task.id, subtask.id, {
        isCompleted: !subtask.isCompleted,
      });
      setItems((current) => current?.map((item) => item.id === updated.id ? updated : item) ?? null);
    }, "Erro ao atualizar subtarefa.");
  }

  function removeSubtask(subtaskId: string) {
    void run(async () => {
      await deleteSubTask(task.id, subtaskId);
      setItems((current) => current?.filter((item) => item.id !== subtaskId) ?? null);
    }, "Erro ao excluir subtarefa.");
  }

  function moveSubtask(sourceId: string, targetId: string) {
    if (!items || sourceId === targetId || saving) return;
    const reordered = [...items];
    const from = reordered.findIndex((item) => item.id === sourceId);
    const to = reordered.findIndex((item) => item.id === targetId);
    if (from < 0 || to < 0) return;
    const [moved] = reordered.splice(from, 1);
    reordered.splice(to, 0, moved);
    const previous = items;
    const ordered = reordered.map((item, position) => ({ ...item, position }));
    setItems(ordered);
    void run(async () => {
      try {
        await reorderSubTasks(task.id, ordered.map((item) => item.id));
      } catch (err) {
        setItems(previous);
        throw err;
      }
      setDraggedId(null);
    }, "Erro ao reordenar subtarefas.");
  }

  return (
    <>
    <tr>
      <td className="task-title-cell">
        <button
          className="task-checklist__toggle"
          type="button"
          aria-expanded={expanded}
          aria-label={`${expanded ? "Recolher" : "Expandir"} subtarefas de ${task.title}`}
          onClick={() => void toggleChecklist()}
        >
          {expanded ? <ChevronDown size={16} /> : <ChevronRight size={16} />}
        </button>
        <button className="task-checklist__title" type="button" onClick={() => void toggleChecklist()}>
          {task.title}
        </button>
      </td>

      <td>
        {task.description ? (
          <span className="task-description" title={task.description}>
            {task.description}
          </span>
        ) : (
          <span className="muted-cell">Sem descrição</span>
        )}
      </td>

      <td>
        <TaskBadge type="priority" value={task.priority} />
      </td>

      <td className="task-status-cell">
        <TaskBadge type="status" value={task.status} />
      </td>

      <td>{formatDueDate(task.dueDate)}</td>
      <td>{task.category?.name ?? "Sem categoria"}</td>

      <td className="task-actions">
        <TaskActionsMenu
          taskTitle={task.title}
          status={task.status}
          isDeleteDisabled={isDeleteDisabled}
          isDeleting={isDeleting}
          isStatusDisabled={isStatusDisabled}
          isStatusUpdating={isStatusUpdating}
          onEdit={onEdit}
          onDelete={onDelete}
          onChangeStatus={(nextStatus) =>
            onChangeStatus(task.id, nextStatus)
          }
        />
      </td>
    </tr>
    {expanded && (
      <tr className="task-checklist__row">
        <td colSpan={7}>
          <section className="task-checklist" aria-label={`Subtarefas de ${task.title}`}>
            {loading ? (
              <p role="status">Carregando subtarefas...</p>
            ) : (
              <>
                {items?.length ? (
                  <ul className="task-checklist__items">
                    {items.map((item) => (
                      <li
                        key={item.id}
                        onDragOver={(event) => event.preventDefault()}
                        onDrop={(event) => {
                          event.preventDefault();
                          if (draggedId) moveSubtask(draggedId, item.id);
                        }}
                      >
                        <button
                          className="task-checklist__grip"
                          type="button"
                          draggable={!saving}
                          aria-label={`Arraste para reordenar ${item.title}`}
                          onDragStart={() => setDraggedId(item.id)}
                          onDragEnd={() => setDraggedId(null)}
                          onKeyDown={(event) => {
                            if (event.key !== "ArrowUp" && event.key !== "ArrowDown") return;
                            event.preventDefault();
                            const index = items.findIndex((entry) => entry.id === item.id);
                            const target = items[index + (event.key === "ArrowUp" ? -1 : 1)];
                            if (target) moveSubtask(item.id, target.id);
                          }}
                        >
                          <GripVertical size={17} />
                        </button>
                        <button
                          className={`task-checklist__check${item.isCompleted ? " is-completed" : ""}`}
                          type="button"
                          aria-label={`${item.isCompleted ? "Reabrir" : "Concluir"} ${item.title}`}
                          aria-pressed={item.isCompleted}
                          disabled={saving}
                          onClick={() => toggleSubtask(item)}
                        >
                          {item.isCompleted && <Check size={14} />}
                        </button>
                        <span className={item.isCompleted ? "is-completed" : undefined}>{item.title}</span>
                        <button
                          className="task-checklist__remove"
                          type="button"
                          aria-label={`Excluir ${item.title}`}
                          disabled={saving}
                          onClick={() => removeSubtask(item.id)}
                        >
                          <Trash2 size={15} />
                        </button>
                      </li>
                    ))}
                  </ul>
                ) : (
                  <p className="task-checklist__empty">Nenhuma subtarefa ainda.</p>
                )}
                <form className="task-checklist__add" onSubmit={(event) => void addSubtask(event)}>
                  <input
                    value={newTitle}
                    onChange={(event) => setNewTitle(event.target.value)}
                    placeholder="Adicionar subtarefa"
                    aria-label="Título da nova subtarefa"
                    disabled={saving}
                  />
                  <button type="submit" aria-label="Adicionar subtarefa" disabled={saving}>
                    <Plus size={17} />
                    Adicionar
                  </button>
                </form>
              </>
            )}
            {error && <p className="task-checklist__error" role="alert">{error}</p>}
          </section>
        </td>
      </tr>
    )}
    </>
  );
}