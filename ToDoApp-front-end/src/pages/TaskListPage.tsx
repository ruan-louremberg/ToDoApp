import { useState } from "react";
import { useTasks } from "../hooks/useTasks";

export function TaskListPage() {
  const [ search, setSearch ] = useState("");
  const [ status, setStatus ] = useState("");
  const [ priority, setPriority ] = useState("");
  

  const { data, loading, error, reload } = useTasks({ search, status, priority});
  

  return (
    <main>
      <h1>Minhas Tarefas</h1>

      <div className="filters">
        <input 
        type="text"
        placeholder="buscar por título..."
        value={search}
        onChange={(e) => setSearch(e.target.value)} />

        <select value={status} onChange={(e) =>
          setStatus(e.target.value)
        }>
          <option value="">Todos os Status</option>
          <option value="Pending">Pendente</option>
          <option value="InProgress">Em Progresso</option>
          <option value="Completed">Concluída</option>
        </select>

        <select value={priority} onChange={(e) => setPriority(e.target.value)}>
          <option value="">Todas as Prioridades</option>
          <option value="Low">Baixa</option>
          <option value="Medium">Média</option>
          <option value="High">Alta</option>
        </select>
      </div>

      {data?.items.length === 0 ? (
        <p>Nenhuma tarefa cadastrada ainda.</p>
      ) : (
        <ul>
          {data?.items.map((task) => (
            <li key={task.id}>
              <strong>{task.title}</strong> - {task.description}
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}