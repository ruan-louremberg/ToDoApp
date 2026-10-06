interface ConfirmDeleteModalProps {
  isOpen: boolean;
  taskTitle?: string;
  itemName?: string;
  itemType?: string;
  isLoading?: boolean;
  error?: string | null;
  onClose: () => void;
  onConfirm: () => void | Promise<void>;
}

export function ConfirmDeleteModal({
  isOpen,
  taskTitle,
  itemName = taskTitle ?? "",
  itemType = "tarefa",
  isLoading = false,
  error,
  onClose,
  onConfirm,
}: ConfirmDeleteModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div
      className="modal-backdrop"
      onClick={() => {
        if (!isLoading) onClose();
      }}
    >
      <div className="modal confirm-modal" onClick={(event) => event.stopPropagation()}>
        <h2>Excluir {itemType}</h2>

        <p className="confirm-modal__message">
          Tem certeza que deseja excluir a {itemType}{" "}
          <strong>{itemName}</strong>?
        </p>
        {error && <p role="alert">{error}</p>}

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
