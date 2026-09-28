import { useTaskSummary } from "../../../hooks/useTaskSummary";
import { SummaryCard } from "./SummaryCard";

export function SummaryPanel() {
  const {
    summary,
    isLoading,
    error
  } = useTaskSummary();

  if (isLoading && !summary) {
    return (
      <section className="summary-panel">
        <h2>Resumo</h2>

        <p>Carregando resumo...</p>
      </section>
    );
  }

  if (!summary) {
    return (
      <section className="summary-panel">
        <h2>Resumo</h2>

        <p>
          Não foi possível carregar o resumo.
        </p>
      </section>
    );
  }

  return (
    <section className="summary-panel">
      <div className="summary-panel__header">
        <h2>Resumo</h2>

        {isLoading && (
          <span>Atualizando...</span>
        )}
      </div>

      <div className="summary-panel__grid">
        <SummaryCard
          title="Total"
          value={summary.totalTasks}
        />

        <SummaryCard
          title="Pendentes"
          value={summary.status.pending}
        />

        <SummaryCard
          title="Em andamento"
          value={summary.status.inProgress}
        />

        <SummaryCard
          title="Concluídas"
          value={summary.status.completed}
        />

        <SummaryCard
          title="Atrasadas"
          value={summary.delays.overdue}
        />
      </div>

      {error && (
        <p className="summary-panel__warning">
          Não foi possível atualizar os dados.
          Tentaremos novamente automaticamente.
        </p>
      )}
    </section>
  );
}