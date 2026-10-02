interface TaskListErrorStateProps {
  message: string;
}

export function TaskListLoadingState() {
  return <p role="status">Carregando tarefas...</p>;
}

export function CategoriesLoadingState() {
  return <p role="status">Carregando categorias...</p>;
}

export function TaskListErrorState({ message }: TaskListErrorStateProps) {
  return <p role="alert">{message}</p>;
}

export function TaskListEmptyState() {
  return <p>Nenhuma tarefa cadastrada ainda.</p>;
}