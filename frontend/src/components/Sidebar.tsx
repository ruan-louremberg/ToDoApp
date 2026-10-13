import { useEffect } from "react";
import { NavLink } from "react-router-dom";
import type { AuthUser } from "../api/auth";

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
  user: AuthUser;
  onLogout: () => void;
}

export function Sidebar({ isOpen, onClose, user, onLogout }: SidebarProps) {
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
          <NavLink
            to="/trash"
            onClick={onClose}
            className={({ isActive }) =>
              `sidebar__link${isActive ? " sidebar__link--active" : ""}`
            }
          >
            <span className="sidebar__icon sidebar__icon--trash" aria-hidden="true">
              🗑️
            </span>
            <span>Lixeira</span>
            <span className="sidebar__link-arrow" aria-hidden="true">›</span>
          </NavLink>
        </nav>
        <div className="sidebar__note">
          <span className="sidebar__note-mark" aria-hidden="true">✦</span>
          <p>Pequenos passos também levam longe.</p>
        </div>
        <div className="sidebar__account">
          <span className="sidebar__avatar" aria-hidden="true">
            {user.name.trim().charAt(0).toLocaleUpperCase("pt-BR")}
          </span>
          <div className="sidebar__account-info">
            <strong title={user.name}>{user.name}</strong>
            <span title={user.email}>{user.email}</span>
          </div>
          <button
            className="sidebar__logout"
            type="button"
            onClick={onLogout}
            aria-label="Sair da conta"
            title="Sair"
          >
            Sair
          </button>
        </div>
      </aside>
    </>
  );
}
