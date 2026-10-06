import { useEffect } from "react";
import { NavLink } from "react-router-dom";

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

export function Sidebar({ isOpen, onClose }: SidebarProps) {
  useEffect(() => {
    if (!isOpen) return;

    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") onClose();
    }

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [isOpen, onClose]);

  return (
    <>
      {isOpen && (
        <button
          className="sidebar-backdrop"
          type="button"
          aria-label="Fechar menu"
          onClick={onClose}
        />
      )}
      <aside
        className={`sidebar${isOpen ? " sidebar--open" : ""}`}
        id="main-sidebar"
        aria-label="Navegação principal"
        aria-hidden={!isOpen}
      >
        <p className="sidebar__eyebrow">   TODO.</p>
        <nav className="sidebar__nav">
          <NavLink
            to="/tasks"
            onClick={onClose}
            className={({ isActive }) =>
              `sidebar__link${isActive ? " sidebar__link--active" : ""}`
            }
          >
            <span className="sidebar__icon" aria-hidden="true">✓</span>
            <span>Tarefas</span>
            <span className="sidebar__link-arrow" aria-hidden="true">›</span>
          </NavLink>
          <NavLink
            to="/categories"
            onClick={onClose}
            className={({ isActive }) =>
              `sidebar__link${isActive ? " sidebar__link--active" : ""}`
            }
          >
            <span className="sidebar__icon sidebar__icon--categories" aria-hidden="true">
              ◈
            </span>
            <span>Categorias</span>
            <span className="sidebar__link-arrow" aria-hidden="true">›</span>
          </NavLink>
        </nav>
        <div className="sidebar__note">
          <span className="sidebar__note-mark" aria-hidden="true">✦</span>
          <p>Pequenos passos também levam longe.</p>
        </div>
      </aside>
    </>
  );
}
