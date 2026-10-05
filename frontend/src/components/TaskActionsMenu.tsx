import { createPortal } from "react-dom";
import { useEffect, useLayoutEffect, useRef, useState } from "react";

interface TaskActionsMenuProps {
  isDeleteDisabled: boolean;
  isDeleting: boolean;
  onEdit: () => void;
  onDelete: () => void;
}

export function TaskActionsMenu({
  isDeleteDisabled,
  isDeleting,
  onEdit,
  onDelete,
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

  return (
    <div className="task-actions-menu">
      <button
        type="button"
        className="task-actions-menu__trigger"
        ref={triggerRef}
        aria-label="Abrir ações da tarefa"
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
