import { useState } from "react";
import { TrashTaskRow } from "../components/TrashTaskRow";
import { ConfirmActionModal } from "../components/ConfirmActionModal";
import { Pagination } from "../components/Pagination";
import {
  TaskListErrorState,
  TaskListLoadingState,
} from "../components/taskListStates";
import { useRestoreTask } from "../hooks/useRestoreTask";
import { useTrashTasks } from "../hooks/useTrashTasks";
import type { ListTrashTasks } from "../types/task";

export function TrashPage() {
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const { data, loading, error, fetchTrashTasks } = useTrashTasks(currentPage, pageSize);
  const [taskToRestore, setTaskToRestore] = useState<ListTrashTasks | null>(null);
  const {
    restore,
    restoringTaskId,
    error: restoreError,
  } = useRestoreTask();

  async function handleRestore(taskId: string) {
    const restored = await restore(taskId);

    if (restored) {
      await fetchTrashTasks();
      setTaskToRestore(null);
    }
  }

  return (
    <main className="trash-page">
      <header className="trash-page__header">
        <div>
          <h1>Lixeira</h1>
          <p>Tarefas excluídas ficam aqui até serem restauradas.</p>
        </div>

        {data && (
          <span className="trash-page__count">
            {data.totalItems} {data.totalItems === 1 ? "tarefa" : "tarefas"}
          </span>
        )}
      </header>

      {restoreError && (
        <p className="trash-page__error" role="alert">
          {restoreError}
        </p>
      )}

      {loading && !data ? (
        <TaskListLoadingState />
      ) : error && !data ? (
        <div className="trash-page__error-state">
          <TaskListErrorState message={error} />
        </div>
      ) : data?.items.length === 0 ? (
        <p className="trash-page__empty">A lixeira está vazia.</p>
      ) : (
        <div className="task-table-wrapper" aria-busy={loading}>
          {loading && (
            <p className="task-list-refreshing" role="status">
              Atualizando lixeira...
            </p>
          )}

          <table className="task-table trash-task-table">
            <thead>
              <tr>
                <th scope="col">Título</th>
                <th scope="col">Descrição</th>
                <th scope="col">Prioridade</th>
                <th scope="col">Status</th>
                <th scope="col">Categoria</th>
                <th scope="col">Excluída em</th>
                <th scope="col">Ações</th>
              </tr>
            </thead>
            <tbody>
              {data?.items.map((task) => (
                <TrashTaskRow
                  key={task.id}
                  task={task}
                  isRestoring={restoringTaskId === task.id}
                  isRestoreDisabled={restoringTaskId !== null}
                  onRestore={(taskId) => {
                    const selectedTask = data?.items.find((task) => task.id === taskId);
                    if (selectedTask) {
                      setTaskToRestore(selectedTask);
                    }
                  }}
                />
              ))}
            </tbody>
          </table>
        </div>
      )}
      {data && data.items.length > 0 && (
        <Pagination
          currentPage={data.page}
          totalPages={data.totalPages}
          pageSize={pageSize}
          itemLabel="tarefas na lixeira"
          disabled={loading}
          onPageChange={setCurrentPage}
          onPageSizeChange={(nextPageSize) => {
            if (nextPageSize < 1 || nextPageSize > 100) return;
            setCurrentPage(1);
            setPageSize(nextPageSize);
          }}
        />
      )}

      <ConfirmActionModal
        isOpen={taskToRestore !== null}
        title="Restaurar tarefa"
        message={`Deseja restaurar "${taskToRestore?.title ?? ""}"? Ela voltará para a lista principal.`}
        confirmLabel="Restaurar"
        loadingLabel="Restaurando..."
        confirmButtonClassName="restore-button"
        isLoading={restoringTaskId !== null}
        onClose={() => setTaskToRestore(null)}
        onConfirm={() => {
          if (taskToRestore) {
            return handleRestore(taskToRestore.id);
          }
        }}
      />
    </main>
  );
}