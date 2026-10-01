const API_URL = import.meta.env.VITE_API_URL;

import type { Category } from "../types/task";

interface CategoriesResponse {
	items: Category[];
}

export async function getCategories(): Promise<Category[]> {
	const response = await fetch(`${API_URL}/api/categories?page=1&pageSize=100`);

	if (!response.ok) {
		throw new Error("Não foi possível carregar as categorias.");
	}

	const data = (await response.json()) as CategoriesResponse;
	return data.items;
}
