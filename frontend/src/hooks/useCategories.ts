import { useEffect, useState } from "react";
import { getCategories } from "../api/category";
import type { Category } from "../types/task";

export function useCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    async function loadCategories() {
      try {
        const result = await getCategories();
        if (active) setCategories(result);
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
