export interface CategoryResponse {
  id: string;
  name: string;
  color: string;
}

export interface CreateCategoryRequest {
  name: string;
  color: string;
}

export interface UpdateCategoryRequest {
  name: string;
  color: string;
}

export interface ListCategoriesRequest {
  name?: string;
  color?: string;
  page?: number;
  pageSize?: number;
}

export interface ListCategoriesResponse {
  items: CategoryResponse[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
}