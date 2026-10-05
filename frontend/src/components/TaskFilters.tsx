interface TaskFiltersProps {
  search: string;
  status: string;
  priority: string;
  onSearchChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onPriorityChange: (value: string) => void;
}

export function TaskFilters({
  search,
  status,
  priority,
  onSearchChange,
  onStatusChange,
  onPriorityChange,
}: TaskFiltersProps) {
  return (
    <div className="filters">
      <input
        type="text"
        placeholder="buscar por título..."
        aria-label="Buscar por título"
        value={search}
        onChange={(event) => onSearchChange(event.target.value)}
      />

      <select
        aria-label="Filtrar por status"
        value={status}
        onChange={(event) => onStatusChange(event.target.value)}
      >
        <option value="">Todos os Status</option>
        <option value="0">Pendente</option>
        <option value="1">Em Progresso</option>
        <option value="2">Concluída</option>
      </select>

      <select
        aria-label="Filtrar por prioridade"
        value={priority}
        onChange={(event) => onPriorityChange(event.target.value)}
      >
        <option value="">Todas as Prioridades</option>
        <option value="0">Baixa</option>
        <option value="1">Média</option>
        <option value="2">Alta</option>
      </select>
    </div>
  );
}