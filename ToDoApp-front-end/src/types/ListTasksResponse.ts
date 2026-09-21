import type { Task } from "./task";

export interface ListTasksResponse {

    items: Task[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
}