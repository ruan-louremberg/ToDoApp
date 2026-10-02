interface ConfirmDeleteModalProps {
  isOpen: boolean;
  taskTitle: string;
  isLoading?: boolean;
  onClose: () => void;
  onConfirm: () => void | Promise<void>;
}

export function ConfirmDeleteModal({
  isOpen,
  taskTitle,
  isLoading = false,
  onClose,
  onConfirm,
}: ConfirmDeleteModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal confirm-modal" onClick={(event) => event.stopPropagation()}>
        <h2>Excluir tarefa</h2>

        <p className="confirm-modal__message">
          Tem certeza que deseja excluir <strong>{taskTitle}</strong>?
        </p>

        <div className="modal-actions">
          <button
            type="button"
            className="secondary-button"
            onClick={onClose}
            disabled={isLoading}
          >
            Cancelar
          </button>

          <button
            type="button"
            className="danger-button"
            onClick={() => void onConfirm()}
            disabled={isLoading}
          >
            {isLoading ? "Excluindo..." : "Excluir"}
          </button>
        </div>
      </div>
    </div>
  );
}
