import { createPortal } from "react-dom";
import { useEffect, useLayoutEffect, useRef, useState } from "react";
import type { TaskStatusDto } from "../types/task";

interface TaskActionsMenuProps {
  taskTitle: string;
  status: TaskStatusDto | null;
  isDeleteDisabled: boolean;
  isDeleting: boolean;
  isStatusDisabled: boolean;
  isStatusUpdating: boolean;
  onEdit: () => void;
  onDelete: () => void;
  onChangeStatus: (status: TaskStatusDto) => Promise<boolean>;
}

const STATUS_ACTIONS: Partial<
  Record<TaskStatusDto, { label: string; nextStatus: TaskStatusDto; isComplete?: boolean }[]>
> = {
  0: [{ label: "Iniciar tarefa", nextStatus: 1 }],
  1: [
    { label: "Voltar para pendente", nextStatus: 0 },
    { label: "Concluir tarefa", nextStatus: 2, isComplete: true },
  ],
  2: [{ label: "Reabrir (em progresso)", nextStatus: 1 }],
};

export function TaskActionsMenu({
  taskTitle,
  status,
  isDeleteDisabled,
  isDeleting,
  isStatusDisabled,
  isStatusUpdating,
  onEdit,
  onDelete,
  onChangeStatus,
}: TaskActionsMenuProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [menuPosition, setMenuPosition] = useState({ top: 0, left: 0 });
  const triggerRef = useRef<HTMLButtonElement>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isOpen) return;

    function handleDocumentClick(event: MouseEvent) {
      const target = event.target as Node;
      if (
        !triggerRef.current?.contains(target) &&
        !dropdownRef.current?.contains(target)
      ) {
        setIsOpen(false);
      }
    }

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setIsOpen(false);
      }
    }

    document.addEventListener("mousedown", handleDocumentClick);
    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("mousedown", handleDocumentClick);
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [isOpen]);

  useLayoutEffect(() => {
    if (!isOpen) return;

    function positionMenu() {
      const trigger = triggerRef.current;
      const dropdown = dropdownRef.current;

      if (!trigger || !dropdown) return;

      const triggerRect = trigger.getBoundingClientRect();
      const dropdownRect = dropdown.getBoundingClientRect();
      const spacing = 6;
      const canOpenBelow =
        triggerRect.bottom + spacing + dropdownRect.height <= window.innerHeight;
      const top = canOpenBelow
        ? triggerRect.bottom + spacing
        : triggerRect.top - dropdownRect.height - spacing;
      const left = Math.min(
        triggerRect.right - dropdownRect.width,
        window.innerWidth - dropdownRect.width - spacing
      );

      setMenuPosition({
        top: Math.max(spacing, top),
        left: Math.max(spacing, left),
      });
    }

    const frameId = requestAnimationFrame(positionMenu);
    window.addEventListener("resize", positionMenu);
    window.addEventListener("scroll", positionMenu, true);

    return () => {
      cancelAnimationFrame(frameId);
      window.removeEventListener("resize", positionMenu);
      window.removeEventListener("scroll", positionMenu, true);
    };
  }, [isOpen]);

  function handleEdit() {
    setIsOpen(false);
    onEdit();
  }

  function handleDelete() {
    setIsOpen(false);
    onDelete();
  }

  async function handleStatusChange(nextStatus: TaskStatusDto) {
    const updated = await onChangeStatus(nextStatus);

    if (updated) {
      setIsOpen(false);
    }
  }

  return (
    <div className="task-actions-menu">
      <button
        type="button"
        className="task-actions-menu__trigger"
        ref={triggerRef}
        aria-label={`Ações da tarefa ${taskTitle}`}
        aria-haspopup="menu"
        aria-expanded={isOpen}
        onClick={() => setIsOpen((current) => !current)}
      >
        <span aria-hidden="true">•••</span>
      </button>

      {isOpen &&
        createPortal(
          <div
            className="task-actions-menu__dropdown"
            ref={dropdownRef}
            role="menu"
            style={{
              top: menuPosition.top,
              left: menuPosition.left,
            }}
          >
            {status !== null &&
              STATUS_ACTIONS[status]?.map((action) => (
                <button
                  key={action.nextStatus}
                  type="button"
                  role="menuitem"
                  className={
                    action.isComplete
                      ? "task-actions-menu__status--complete"
                      : undefined
                  }
                  disabled={isStatusDisabled}
                  onClick={() => void handleStatusChange(action.nextStatus)}
                >
                  {isStatusUpdating ? "Salvando..." : action.label}
                </button>
              ))}
            {status !== null && <div className="task-actions-menu__separator" role="separator" />}
            <button type="button" role="menuitem" onClick={handleEdit}>
              Editar
            </button>
            <button
              type="button"
              role="menuitem"
              className="task-actions-menu__delete"
              disabled={isDeleteDisabled}
              onClick={handleDelete}
            >
              {isDeleting ? "Excluindo..." : "Excluir"}
            </button>
          </div>,
          document.body
        )}
    </div>
  );
}
