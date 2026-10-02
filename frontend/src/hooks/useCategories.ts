import { useEffect, useState } from "react";
import { getCategories, } from "../api/category";
import type { CategoryResponse } from "../types/category";

export function useCategories() {
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    async function loadCategories() {
      try {
        const result = await getCategories();
        
        if (active) setCategories(result.items);

      } catch (err) {
        if (active) {
          setError(
            err instanceof Error
              ? err.message
              : "Não foi possível carregar as categorias."
          );
        }
      } finally {
        if (active) setLoading(false);
      }
    }

    void loadCategories();

    return () => {
      active = false;
    };
  }, []);

  return { categories, loading, error };
}
