interface PaginationProps {
  currentPage: number;
  totalPages: number;
  pageSize: number;
  disabled?: boolean;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export function Pagination({
  currentPage,
  totalPages,
  pageSize,
  disabled = false,
  onPageChange,
  onPageSizeChange,
}: PaginationProps) {
  const canGoPrevious = currentPage > 1;
  const canGoNext = currentPage < totalPages;

  function handlePrevious() {
    if (canGoPrevious) {
      onPageChange(currentPage - 1);
    }
  }

  function handleNext() {
    if (canGoNext) {
      onPageChange(currentPage + 1);
    }
  }

  return (
    <nav className="pagination" aria-label="Paginação das tarefas">
      <button
        type="button"
        onClick={handlePrevious}
        disabled={disabled || !canGoPrevious}
        aria-label="Ir para a página anterior"
      >
        ←
      </button>

      <span>
        Página {currentPage} de {totalPages}
      </span>

      <button
        type="button"
        onClick={handleNext}
        disabled={disabled || !canGoNext}
        aria-label="Ir para a próxima página"
      >
        →
      </button>

      <select
        className="pagination__page-size"
        aria-label="Quantidade de tarefas por página"
        value={pageSize}
        disabled={disabled}
        onChange={(event) => onPageSizeChange(Number(event.target.value))}
      >
        <option value={10}>10</option>
        <option value={20}>20</option>
        <option value={50}>50</option>
      </select>
    </nav>
  );
}
