interface ConfirmActionModalProps {
  isOpen: boolean;
  title: string;
  message: string;
  confirmLabel: string;
  loadingLabel: string;
  confirmButtonClassName?: string;
  isLoading?: boolean;
  onClose: () => void;
  onConfirm: () => void | Promise<void>;
}

export function ConfirmActionModal({
  isOpen,
  title,
  message,
  confirmLabel,
  loadingLabel,
  confirmButtonClassName = "danger-button",
  isLoading = false,
  onClose,
  onConfirm,
}: ConfirmActionModalProps) {
  if (!isOpen) {
    return null;
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal confirm-modal" onClick={(event) => event.stopPropagation()}>
        <h2>{title}</h2>

        <p className="confirm-modal__message">
          {message}
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
            className={confirmButtonClassName}
            onClick={() => void onConfirm()}
            disabled={isLoading}
          >
            {isLoading ? loadingLabel : confirmLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
