import type { TaskResponseList } from "./task";

export interface ListTasksResponse {

    items: TaskResponseList[];
    page: number;
    pageSize: number;
    totalItems: number;
    totalPages: number;
}