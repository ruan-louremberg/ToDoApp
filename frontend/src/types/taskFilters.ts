import type { TaskPriorityDto, TaskStatusDto } from "./task";

export interface TaskFilters {
  search?: string;
  status?: TaskStatusDto;
  priority?: TaskPriorityDto;
  sortBy?: string;
  sortDirection?: "asc" | "desc";
  categoryId?: string;
  page?: number;
  pageSize?: number;
}

export type ListTasksRequest = TaskFilters;