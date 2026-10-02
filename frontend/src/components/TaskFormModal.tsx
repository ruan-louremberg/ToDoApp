import type { SubmitEvent } from "react";
import { useEffect, useState } from "react";
import type { Priority } from "../types/task";
import type { TaskFormData } from "../types/taskForm";
import type { CategoryResponse } from "../types/category";

interface TaskFormModalProps {
  isOpen: boolean;
  title: string;
  submitLabel: string;
  initialData?: TaskFormData;
  saving: boolean;
  error: string | null;
  categories: CategoryResponse[];
  categoriesLoading: boolean;
  onClose: () => void;
  onSubmit: (data: TaskFormData) => Promise<void>;
}

const emptyForm: TaskFormData = {
  title: "",
  description: "",
  priority: 1,
  dueDate: "",
  categoryId: "",
};

export function TaskFormModal({
  isOpen,
  title,
  submitLabel,
  initialData,
  saving,
  error,
  categories,
  categoriesLoading,
  onClose,
  onSubmit,
}: TaskFormModalProps) {

  console.log(categories)
  const [formData, setFormData] = useState<TaskFormData>(emptyForm);

  useEffect(() => {
    if (isOpen) {
      setFormData(initialData ?? emptyForm);
    }
  }, [initialData, isOpen]);

  if (!isOpen) {
    return null;
  }

  function updateField<Key extends keyof TaskFormData>(
    field: Key,
    value: TaskFormData[Key]
  ) {
    setFormData((current) => ({
      ...current,
      [field]: value,
    }));
  }

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    await onSubmit(formData);
  }

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <h2>{title}</h2>

        <form onSubmit={handleSubmit}>
          <label>
            Título
            <input
              value={formData.title}
              onChange={(event) => updateField("title", event.target.value)}
              minLength={3}
              maxLength={120}
              required
            />
          </label>

          <label>
            Descrição
            <textarea
              value={formData.description}
              onChange={(event) =>
                updateField("description", event.target.value)
              }
              maxLength={1000}
            />
          </label>

          <label>
            Prioridade
            <select
              value={formData.priority}
              onChange={(event) =>
                updateField("priority", Number(event.target.value) as Priority)
              }
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
              value={formData.dueDate}
              onChange={(event) => updateField("dueDate", event.target.value)}
            />
          </label>

          <label>
            Categoria
            <select
              value={formData.categoryId}
              disabled={categoriesLoading}
              onChange={(event) => updateField("categoryId", event.target.value)}
            >
              <option value="">
                {categoriesLoading ? "Carregando categorias..." : "Sem categoria"}
              </option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </label>

          {error && <p>{error}</p>}

          <button type="button" onClick={onClose} disabled={saving}>
            Cancelar
          </button>

          <button type="submit" disabled={saving}>
            {saving ? "Salvando..." : submitLabel}
          </button>
        </form>
      </div>
    </div>
  );
}
