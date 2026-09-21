import { useTasks } from "./hooks/useTasks";

export function App() {
  const { data, loading, error, reload } = useTasks();

  if (loading) return <p>Carregando tarefas...</p>;
  if (error) return <p>{error}</p>;

  return (
    <main>
      <h1>Minhas Tarefas</h1>
      <ul>
        {data?.items.map((task) => (
          <li key={task.id}>
            <strong>{task.title}</strong> - {task.description}
          </li>
        ))}
      </ul>
    </main>
  );
}