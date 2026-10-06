import { useCallback, useState } from "react";
import { Edit2, Plus, Search, Tags, Trash2 } from "lucide-react";
import { deleteCategory } from "../api/category";
import { getTasks } from "../api/tasks";
import { CategoryFormModal } from "../components/CategoryFormModal";
import { ConfirmDeleteModal } from "../components/ConfirmDeleteModal";
import { Pagination } from "../components/Pagination";
import { useCategories } from "../hooks/useCategories";
import { useDebounce } from "../hooks/useDebounce";
import type { CategoryResponse } from "../types/category";

export function CategoriesPage() {
  const [search, setSearch] = useState("");
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [isFormModalOpen, setIsFormModalOpen] = useState(false);
  const [categoryToEdit, setCategoryToEdit] = useState<CategoryResponse | null>(null);
  const [categoryToDelete, setCategoryToDelete] = useState<CategoryResponse | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [checkingCategoryId, setCheckingCategoryId] = useState<string | null>(null);
  const [linkedTaskCount, setLinkedTaskCount] = useState<number | undefined>();
  const [checkDeleteError, setCheckDeleteError] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const debouncedSearch = useDebounce(search, 300);
  const { data, loading, error, reload } = useCategories({
    name: debouncedSearch.trim() || undefined,
    page: currentPage,
    pageSize,
  });
  const categories = data?.items ?? [];

  function openCreateModal() {
    setCategoryToEdit(null);
    setIsFormModalOpen(true);
  }

  function openEditModal(category: CategoryResponse) {
    setCategoryToEdit(category);
    setIsFormModalOpen(true);
  }

  const closeFormModal = useCallback(() => {
    setIsFormModalOpen(false);
    setCategoryToEdit(null);
  }, []);

  function handleCategorySaved() {
    setSearch("");
    setCurrentPage(1);
    void reload();
  }

  function handleSearchChange(value: string) {
    setSearch(value);
    setCurrentPage(1);
  }

  function handlePageSizeChange(nextPageSize: number) {
    if (!Number.isInteger(nextPageSize) || nextPageSize < 1 || nextPageSize > 100) {
      return;
    }

    setCurrentPage(1);
    setPageSize(nextPageSize);
  }

  async function handleDeleteCategory() {
    if (!categoryToDelete) return;

    setDeleting(true);
    setDeleteError(null);

    try {
      await deleteCategory(categoryToDelete.id);
      setCategoryToDelete(null);
      setSearch("");
      setCurrentPage(1);
      await reload();
    } catch (err) {
      setDeleteError(
        err instanceof Error
          ? err.message
          : "Não foi possível excluir a categoria."
      );
    } finally {
      setDeleting(false);
    }
  }

  async function requestDeleteConfirmation(category: CategoryResponse) {
    setCheckingCategoryId(category.id);
    setCheckDeleteError(null);
    setDeleteError(null);

    try {
      const tasks = await getTasks({
        categoryId: category.id,
        page: 1,
        pageSize: 1,
      });
      setLinkedTaskCount(tasks.totalItems);
      setCategoryToDelete(category);
    } catch (err) {
      setCheckDeleteError(
        err instanceof Error
          ? `Não foi possível verificar as tarefas vinculadas: ${err.message}`
          : "Não foi possível verificar as tarefas vinculadas."
      );
    } finally {
      setCheckingCategoryId(null);
    }
  }

  return (
    <main className="categories-page">
      <header className="categories-page__heading">
        <div>
          <h1>Categorias</h1>
          <p>Organize suas tarefas por categoria.</p>
        </div>
      </header>

      <section className="categories-panel" aria-labelledby="categories-heading">
        {checkDeleteError && (
          <p className="categories-inline-error" role="alert">{checkDeleteError}</p>
        )}
        <div className="categories-panel__header">
          <div className="categories-panel__title">
            <span className="categories-panel__title-icon" aria-hidden="true">
              <Tags size={19} strokeWidth={1.9} />
            </span>
            <div>
              <h2 id="categories-heading">Minhas categorias</h2>
              <p>
                {data
                  ? `${data.totalItems} ${data.totalItems === 1 ? "categoria cadastrada" : "categorias cadastradas"}`
                  : "Gerencie suas categorias"}
              </p>
            </div>
          </div>
          <div className="categories-panel__actions">
            <label className="categories-search">
              <Search className="categories-search__icon" size={17} aria-hidden="true" />
              <span className="visually-hidden">Buscar categorias</span>
              <input
                type="search"
                value={search}
                onChange={(event) => handleSearchChange(event.target.value)}
                placeholder="Buscar categoria..."
              />
            </label>
            <button
              className="category-create-button"
              type="button"
              onClick={openCreateModal}
            >
              <Plus size={18} strokeWidth={2.5} />
              <span>Nova categoria</span>
            </button>
          </div>
        </div>

        {loading && !data ? (
          <p className="categories-state" role="status">Carregando categorias...</p>
        ) : error && !data ? (
          <div className="categories-state categories-state--error" role="alert">
            <p>{error}</p>
            <button type="button" onClick={() => void reload()}>Tentar novamente</button>
          </div>
        ) : categories.length === 0 ? (
          <div className="categories-empty">
            <span className="categories-empty__icon" aria-hidden="true">
              <Tags size={24} strokeWidth={1.7} />
            </span>
            <h3>{debouncedSearch ? "Nenhum resultado encontrado" : "Nenhuma categoria cadastrada"}</h3>
            <p>
              {debouncedSearch
                ? "Tente buscar por outro nome."
                : "Crie uma categoria para organizar suas tarefas."}
            </p>
            {!debouncedSearch && (
              <button
                className="category-create-button category-create-button--empty"
                type="button"
                onClick={openCreateModal}
              >
                <Plus size={18} strokeWidth={2.5} />
                Nova categoria
              </button>
            )}
          </div>
        ) : (
          <>
            {loading && (
              <p className="categories-refreshing" role="status">
                Atualizando categorias...
              </p>
            )}
            {error && <p className="categories-inline-error" role="alert">{error}</p>}
            <div className="category-grid">
              {categories.map((category) => (
                <article className="category-card" key={category.id}>
                  <span
                    className="category-card__swatch"
                    style={{ backgroundColor: category.color }}
                    aria-label={`Cor ${category.color}`}
                    role="img"
                  />
                  <div className="category-card__content">
                    <span className="category-card__label">CATEGORIA</span>
                    <h3 className="category-card__name">
                      <span className="category-card__name-text" title={category.name}>
                        {category.name}
                      </span>
                    </h3>
                    <span className="category-card__color">{category.color}</span>
                  </div>
                  <div className="category-card__actions">
                    <button
                      type="button"
                      aria-label={`Editar categoria ${category.name}`}
                      title="Editar categoria"
                      onClick={() => openEditModal(category)}
                    >
                      <Edit2 size={16} />
                    </button>
                    <button
                      type="button"
                      aria-label={`Excluir categoria ${category.name}`}
                      title={checkingCategoryId === category.id
                        ? "Verificando tarefas vinculadas..."
                        : "Excluir categoria"}
                      disabled={checkingCategoryId !== null}
                      onClick={() => void requestDeleteConfirmation(category)}
                    >
                      {checkingCategoryId === category.id ? (
                        <span aria-hidden="true">…</span>
                      ) : (
                        <Trash2 size={16} />
                      )}
                    </button>
                  </div>
                </article>
              ))}
            </div>
            {data && (
              <Pagination
                currentPage={data.page}
                totalPages={data.totalPages}
                pageSize={pageSize}
                itemLabel="categorias"
                disabled={loading}
                onPageChange={setCurrentPage}
                onPageSizeChange={handlePageSizeChange}
              />
            )}
          </>
        )}
      </section>

      {isFormModalOpen && (
        <CategoryFormModal
          category={categoryToEdit}
          onClose={closeFormModal}
          onSaved={handleCategorySaved}
        />
      )}
      <ConfirmDeleteModal
        isOpen={categoryToDelete !== null}
        itemName={categoryToDelete?.name}
        itemType="categoria"
        linkedTaskCount={linkedTaskCount}
        isLoading={deleting}
        error={deleteError}
        onClose={() => {
          setCategoryToDelete(null);
          setDeleteError(null);
        }}
        onConfirm={handleDeleteCategory}
      />
    </main>
  );
}
