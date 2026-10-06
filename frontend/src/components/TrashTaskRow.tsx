import { TaskBadge } from "./TaskBadge";
import type { ListTrashTasks } from "../types/task";

interface TrashTaskRowProps {
  task: ListTrashTasks;
  isRestoring: boolean;
  isRestoreDisabled: boolean;
  onRestore: (taskId: string) => void;
}

function formatDeletedAt(value: string | null) {
  if (!value) return "Data não informada";

  const date = new Date(`${value.slice(0, 10)}T00:00:00`);

  if (Number.isNaN(date.getTime()) || date.getFullYear() < 1000) {
    return "Data não informada";
  }

  return new Intl.DateTimeFormat("pt-BR").format(date);
}

export function TrashTaskRow({
  task,
  isRestoring,
  isRestoreDisabled,
  onRestore,
}: TrashTaskRowProps) {
  return (
    <tr>
      <td className="task-title-cell">{task.title}</td>

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

      <td>{task.category?.name ?? "Sem categoria"}</td>
      <td>{formatDeletedAt(task.deletedAt)}</td>

      <td className="trash-task-actions">
        <button
          type="button"
          className="restore-button"
          disabled={isRestoreDisabled}
          onClick={() => onRestore(task.id)}
        >
          {isRestoring ? "Restaurando..." : "Restaurar"}
        </button>
      </td>
    </tr>
  );
}