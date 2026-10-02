const API_URL = import.meta.env.VITE_API_URL;

import { throwApiError } from "./tasks";
import type {
	CategoryResponse,
	CreateCategoryRequest,
	ListCategoriesRequest,
	ListCategoriesResponse,
	UpdateCategoryRequest,
} from "../types/category";

export async function getCategories(
	filters: ListCategoriesRequest = {}
	): Promise<ListCategoriesResponse> {
	const params = new URLSearchParams();

	if (filters.name) params.set("name", filters.name);
	if (filters.color) params.set("color", filters.color);
	if (filters.page !== undefined) params.set("page", String(filters.page));
	if (filters.pageSize !== undefined) params.set("pageSize", String(filters.pageSize));

	const query = params.toString();
	const response = await fetch(`${API_URL}/api/categories${query ? `?${query}` : ""}`);

	if (!response.ok) {
		await throwApiError(response, "Erro ao buscar categorias");
	}

	return response.json();
}

export async function createCategory(
	data: CreateCategoryRequest
	): Promise<{ success: boolean; error?: string }> {
	const response = await fetch(`${API_URL}/api/categories`, {
		method: "POST",
		headers: {
			"Content-Type": "application/json",
		},
		body: JSON.stringify(data),
	});

	if (!response.ok) {
		await throwApiError(response, "Erro ao criar categoria");
	}

	return { success: true };
}

export async function updateCategory(
	id: string,
	data: UpdateCategoryRequest
	): Promise<{ success: boolean; error?: string }> {
	const response = await fetch(`${API_URL}/api/categories/${id}`, {
		method: "PATCH",
		headers: {
			"Content-Type": "application/json",
		},
		body: JSON.stringify(data),
	});

	if (!response.ok) {
		await throwApiError(response, "Erro ao atualizar categoria");
	}

	return { success: true };
}

export async function deleteCategory(
	id: string
): Promise<{ success: boolean; error?: string }> {
	const response = await fetch(`${API_URL}/api/categories/${id}`, {
		method: "DELETE",
	});

	if (!response.ok) {
		await throwApiError(response, "Erro ao excluir categoria");
	}

	return { success: true };
}

export async function getDeletedCategories(): Promise<CategoryResponse[]> {
	const response = await fetch(`${API_URL}/api/categories/trash`);

	if (!response.ok) {
		await throwApiError(response, "Erro ao buscar categorias excluídas");
	}

	return response.json();
}

export async function restoreCategory(
	id: string
): Promise<{ success: boolean; error?: string }> {
	const response = await fetch(`${API_URL}/api/categories/${id}/restore`, {
		method: "PATCH",
	});

	if (!response.ok) {
		await throwApiError(response, "Erro ao restaurar categoria");
	}

	return { success: true };
}
