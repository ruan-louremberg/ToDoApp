import  type { SubmitEvent } from "react";
import { useState } from "react";
import { createTask } from "../api/tasks";
import type { CreateTaskData } from "../types/task";

interface CreateTaskModalProps {
  isOpen: boolean;
  onClose: () => void;
  onCreated: () => void;
}

export function CreateTaskModal({
  isOpen,
  onClose,
  onCreated,
}: CreateTaskModalProps) {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [priority, setPriority] = useState(1);
  const [dueDate, setDueDate] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  if (!isOpen) {
    return null;
  }

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError(null);

    const data: CreateTaskData = {
      title,
      description,
      priority,
      dueDate: dueDate || null,
    };

    const result = await createTask(data)

    setSaving(false);

    if (!result.success){
        setError(result.error?? "Erro Inesperado")
        return;
    }
    setTitle("");
    setDescription("");
    setDueDate("");
    setPriority(1);
    onCreated();
    onClose();
  }

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>Nova tarefa</h2>

        <form onSubmit={handleSubmit}>
          <label>
            Título
            <input
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              minLength={3}
              maxLength={120}
              required
            />
          </label>

          <label>
            Descrição
            <textarea
              value={description}
              onChange={(event) => setDescription(event.target.value)}
              maxLength={1000}
            />
          </label>

          <label>
            Prioridade
            <select
              value={priority}
              onChange={(event) => setPriority(Number(event.target.value))}
            >
              <option value={0}>Baixa</option>
              <option value={1}>Média</option>
              <option value={2}>Alta</option>
            </select>
          </label>

          <label>
            Data de vencimento
            <input
              type="date"
              value={dueDate}
              onChange={(event) => setDueDate(event.target.value)}
            />
          </label>

          {error && <p>{error}</p>}

          <button type="button" onClick={onClose}>
            Cancelar
          </button>

          <button type="submit" disabled={saving}>
            {saving ? "Salvando..." : "Criar tarefa"}
          </button>
        </form>
      </div>
    </div>
  );
}