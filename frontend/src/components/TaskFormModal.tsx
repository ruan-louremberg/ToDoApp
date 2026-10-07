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
    <div className="category-modal-backdrop">
      <section
        className="category-modal task-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="task-modal-title"
      >
        <div className="category-modal__heading">
          <span className="category-modal__icon" aria-hidden="true">
            ✓
          </span>
          <div>
            <p className="category-modal__eyebrow">TAREFA</p>
            <h2 id="task-modal-title">{title}</h2>
          </div>
        </div>

        <form className="task-form" onSubmit={handleSubmit}>
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

          {error && <p className="task-form__error" role="alert">{error}</p>}

          <div className="task-form__actions">
            <button
              className="task-form__cancel"
              type="button"
              onClick={onClose}
              disabled={saving}
            >
              Cancelar
            </button>
            <button className="task-form__submit" type="submit" disabled={saving}>
              {saving ? "Salvando..." : submitLabel}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
