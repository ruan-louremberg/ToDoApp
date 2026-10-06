interface ConfirmDeleteModalProps {
  isOpen: boolean;
  taskTitle?: string;
  itemName?: string;
  itemType?: string;
  linkedTaskCount?: number;
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
  linkedTaskCount,
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
          {itemType === "categoria" && linkedTaskCount !== undefined && linkedTaskCount > 0 ? (
            <>
              Esta categoria está vinculada a{" "}
              <strong>
                {linkedTaskCount} {linkedTaskCount === 1 ? "tarefa" : "tarefas"}
              </strong>
              . Tem certeza que deseja excluí-la?
            </>
          ) : (
            <>
              Tem certeza que deseja excluir a {itemType}{" "}
              <strong>{itemName}</strong>?
            </>
          )}
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
