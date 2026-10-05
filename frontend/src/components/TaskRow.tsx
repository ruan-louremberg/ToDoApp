import { TaskActionsMenu } from "./TaskActionsMenu";
import { TaskBadge } from "./TaskBadge";
import type { TaskResponseList, TaskStatusDto } from "../types/task";

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
  );
}