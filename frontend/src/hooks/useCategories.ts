import { useCallback, useEffect, useState } from "react";
import { getCategories } from "../api/category";
import type {
  CategoryResponse,
  ListCategoriesRequest,
  ListCategoriesResponse,
} from "../types/category";

export function useCategories(filters: ListCategoriesRequest = {}) {
  const { name, color, page, pageSize } = filters;
  const [data, setData] = useState<ListCategoriesResponse | null>(null);
  const [reloadVersion, setReloadVersion] = useState(0);
  const [loadedQuery, setLoadedQuery] = useState<string | null>(null);
  const [requestError, setRequestError] = useState<{
    query: string;
    message: string;
  } | null>(null);
  const query = JSON.stringify([name, color, page, pageSize, reloadVersion]);

  useEffect(() => {
    let active = true;

    async function loadCategories() {
      try {
        const result = await getCategories({ name, color, page, pageSize });
        if (active) {
          setData(result);
          setRequestError(null);
        }
      } catch (err) {
        if (active) {
          setRequestError({
            query,
            message:
              err instanceof Error
                ? err.message
                : "Não foi possível carregar as categorias.",
          });
        }
      } finally {
        if (active) setLoadedQuery(query);
      }
    }

    void loadCategories();

    return () => {
      active = false;
    };
  }, [name, color, page, pageSize, query]);

  const reload = useCallback(() => {
    setReloadVersion((version) => version + 1);
  }, []);
  const categories: CategoryResponse[] = data?.items ?? [];
  const loading = loadedQuery !== query;
  const error = requestError?.query === query ? requestError.message : null;

  return { categories, data, loading, error, reload };
}
