import { useEffect, useState, type FormEvent } from "react";
import { createCategory, updateCategory } from "../api/category";
import type { CategoryResponse } from "../types/category";

interface CategoryFormModalProps {
  category: CategoryResponse | null;
  onClose: () => void;
  onSaved: () => void;
}

export function CategoryFormModal({
  category,
  onClose,
  onSaved,
}: CategoryFormModalProps) {
  const [name, setName] = useState(category?.name ?? "");
  const [color, setColor] = useState(category?.color ?? "#8B80F8");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    function handleKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape" && !saving) onClose();
    }

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, [onClose, saving]);

  function closeModal() {
    onClose();
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmedName = name.trim();

    if (trimmedName.length < 2) {
      setError("O nome precisa ter pelo menos 2 caracteres.");
      return;
    }

    if (trimmedName.length > 50) {
      setError("O nome pode ter no máximo 50 caracteres.");
      return;
    }

    setSaving(true);
    setError(null);

    try {
      if (category) {
        await updateCategory(category.id, { name: trimmedName, color });
      } else {
        await createCategory({ name: trimmedName, color });
      }
      onSaved();
      closeModal();
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : `Não foi possível ${category ? "atualizar" : "criar"} a categoria.`
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <div
      className="category-modal-backdrop"
      onMouseDown={(event) => {
        if (event.target === event.currentTarget && !saving) closeModal();
      }}
    >
      <section
        className="category-modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="category-modal-title"
      >
        <div className="category-modal__heading">
          <span className="category-modal__icon" aria-hidden="true">
            <span style={{ backgroundColor: color }} />
          </span>
          <div>
            <p className="category-modal__eyebrow">
              {category ? "PERSONALIZAR" : "NOVA CATEGORIA"}
            </p>
            <h2 id="category-modal-title">
              {category ? "Editar categoria" : "Criar categoria"}
            </h2>
          </div>
        </div>

        <form className="category-form" autoComplete="off" onSubmit={handleSubmit}>
          <label htmlFor="category-name">Nome da categoria</label>
          <input
            id="category-name"
            name="category-title"
            autoFocus
            autoComplete="new-password"
            maxLength={50}
            value={name}
            onChange={(event) => {
              setName(event.target.value);
              if (error) setError(null);
            }}
            placeholder="Ex.: Estudos, Casa, Trabalho"
            spellCheck={false}
            required
          />
          <div className="category-form__color-heading">
            <label htmlFor="category-color">Escolha uma cor</label>
            <span>{color.toUpperCase()}</span>
          </div>
          <div className="category-form__color-control">
            <input
              id="category-color"
              type="color"
              value={color}
              onChange={(event) => setColor(event.target.value)}
              aria-label="Cor da categoria"
            />
            <span>Use uma cor para reconhecer esta categoria rapidamente.</span>
          </div>
          {error && <p className="category-form__error" role="alert">{error}</p>}
          <div className="category-form__actions">
            <button
              className="category-form__cancel"
              type="button"
              disabled={saving}
              onClick={closeModal}
            >
              Cancelar
            </button>
            <button className="category-form__submit" type="submit" disabled={saving}>
              {saving
                ? category ? "Salvando..." : "Criando..."
                : category ? "Salvar alterações" : "Criar categoria"}
            </button>
          </div>
        </form>
      </section>
    </div>
  );
}
