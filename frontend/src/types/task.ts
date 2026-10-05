import type { CategoryResponse } from "./category";

export type Status = 0 | 1 | 2;
export type Priority = 0 | 1 | 2;

export type TaskStatusDto = Status;
export type TaskPriorityDto = Priority;

export interface TaskResponse {
    id: string;
    title: string;
    description: string | null;
    priority: number;
    dueDate: string | null;
    category: CategoryResponse | null;
}

export interface TaskResponseList {
    id: string;
    title: string;
    description: string | null;
    status: TaskStatusDto | null;
    priority: TaskPriorityDto | null;
    dueDate: string | null;
    category: CategoryResponse | null;
}

export interface ChangeTaskStatusRequest {
  status: TaskStatusDto;
}

export interface CompleteTaskResponse {
    id: string;
    title: string;
    description: string | null;
    status: TaskStatusDto;
    priority: TaskPriorityDto;
    dueDate: string | null;
    categoryId: string | null;
    createdAt: string;
    updatedAt: string | null;
    completedAt: string | null;
}

export interface CreateTaskRequest {
    title: string;
    description?: string;
    priority: TaskPriorityDto;
    dueDate?: string | null;
    categoryId?: string | null;
}

export type CreateTaskData = CreateTaskRequest;

export interface UpdateTaskRequest {
    title?: string;
    description?: string | null;
    priority?: TaskPriorityDto | null;
    dueDate?: string | null;
    categoryId?: string | null;
}