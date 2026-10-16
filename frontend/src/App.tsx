import { useCallback, useState } from "react";
import type { ReactNode } from "react";
import {
  BrowserRouter,
  Navigate,
  Outlet,
  Route,
  Routes,
  Link,
  useNavigate,
} from "react-router-dom";
import { Sidebar } from "./components/Sidebar";
import { CategoriesPage } from "./pages/CategoriesPage";
import { LoginPage } from "./pages/LoginPage";
import { RegisterPage } from "./pages/RegisterPage";
import { ForgotPasswordPage } from "./pages/ForgotPasswordPage";
import { ResetPasswordPage } from "./pages/ResetPasswordPage";
import { TaskListPage } from "./pages/TaskListPage";
import { TrashPage } from "./pages/TrashPage";
import { SummaryPanel } from "./features/tasks/summary/SummaryPanel";
import { useTaskSummary } from "./hooks/useTaskSummary";
import { AuthProvider, useAuth } from "./auth/AuthContext";

function RequireAuth() {
  const { user, loading } = useAuth();

  if (loading) {
    return <main className="auth-state">Verificando sua sessão...</main>;
  }

  return user ? <Outlet /> : <Navigate to="/login" replace />;
}

function GuestOnly({ children }: { children: ReactNode }) {
  const { user, loading } = useAuth();

  if (loading) {
    return <main className="auth-state">Verificando sua sessão...</main>;
  }

  return user ? <Navigate to="/tasks" replace /> : children;
}

function TasksPage() {
  const summaryState = useTaskSummary();

  return (
    <>
      <SummaryPanel
        summary={summaryState.summary}
        isLoading={summaryState.isLoading}
        error={summaryState.error}
      />
      <TaskListPage onTasksChanged={summaryState.reload} />
    </>
  );
}

function AppLayout() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);
  const closeSidebar = useCallback(() => setIsSidebarOpen(false), []);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  async function handleLogout() {
    setLogoutError(null);
    try {
      await logout();
      navigate("/login", { replace: true });
    } catch (error) {
      setLogoutError(
        error instanceof Error
          ? error.message
          : "Não foi possível encerrar sua sessão."
      );
    }
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="app-container">
      <header className="header">
        <button
          className="menu-toggle"
          type="button"
          aria-label={isSidebarOpen ? "Fechar menu" : "Abrir menu"}
          aria-expanded={isSidebarOpen}
          aria-controls="main-sidebar"
          onClick={() => setIsSidebarOpen((open) => !open)}
        >
          <span className="menu-toggle__line" />
          <span className="menu-toggle__line" />
          <span className="menu-toggle__line" />
        </button>
        <Link className="header__brand" to="/tasks" aria-label="ToDoApp, tarefas">
          <span className="header__brand-mark" aria-hidden="true">T</span>
          <span>todo<span className="header__brand-accent">.</span></span>
        </Link>
        <span className="header__caption">Organize seu dia, uma tarefa por vez.</span>
      </header>
      <div className="app-shell">
        <Sidebar
          isOpen={isSidebarOpen}
          onClose={closeSidebar}
          user={user}
          onLogout={handleLogout}
        />
        <div className="app-content">
          {logoutError && <p className="sidebar__error" role="alert">{logoutError}</p>}
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route
            path="/login"
            element={<GuestOnly><LoginPage /></GuestOnly>}
          />
          <Route
            path="/register"
            element={<GuestOnly><RegisterPage /></GuestOnly>}
          />
          <Route
            path="/forgot-password"
            element={<GuestOnly><ForgotPasswordPage /></GuestOnly>}
          />
          <Route
            path="/reset-password"
            element={<GuestOnly><ResetPasswordPage /></GuestOnly>}
          />
          <Route element={<RequireAuth />}>
            <Route element={<AppLayout />}>
              <Route index element={<Navigate to="/tasks" replace />} />
              <Route path="/tasks" element={<TasksPage />} />
              <Route path="/categories" element={<CategoriesPage />} />
              <Route path="/trash" element={<TrashPage />} />
              <Route path="*" element={<Navigate to="/tasks" replace />} />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );

}