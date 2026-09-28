import { useTaskSummary } from "../hooks/useTaskSummary";

export function TaskSummaryPage() {
  const { data, loading, error } = useTaskSummary();

  if (loading) return <p>Carregando resumo...</p>;
  if (error) return <p>{error}</p>;

  return (
    <section>
      <h2>Resumo</h2>

      <div>
        <p>Total de tarefas:  <strong>{data?.totalTasks}</strong></p>
        
      </div>

      <div>
        <p>Pendentes:  <strong>{data?.status.pending}</strong></p>
      </div>

      <div>
        <p>Em andamento:   <strong>{data?.status.inProgress}</strong></p>
      </div>

      <div>
        <p>Concluídas:  <strong>{data?.status.completed}</strong></p>
      </div>

      <div>
        <p>Atrasadas:  <strong>{data?.delays.overdue}</strong></p>
      </div>
    </section>
  );
}