import { useCallback, useState } from "react";
import {
  BrowserRouter,
  Navigate,
  Outlet,
  Route,
  Routes,
  Link,
} from "react-router-dom";
import { Sidebar } from "./components/Sidebar";
import { CategoriesPage } from "./pages/CategoriesPage";
import { TaskListPage } from "./pages/TaskListPage";
import { SummaryPanel } from "./features/tasks/summary/SummaryPanel";
import { useTaskSummary } from "./hooks/useTaskSummary";

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
  const closeSidebar = useCallback(() => setIsSidebarOpen(false), []);

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
        />
        <div className="app-content">
          <Outlet />
        </div>
      </div>
    </div>
  );
}

export function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route index element={<Navigate to="/tasks" replace />} />
          <Route path="/tasks" element={<TasksPage />} />
          <Route path="/categories" element={<CategoriesPage />} />
          <Route path="*" element={<Navigate to="/tasks" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );

}