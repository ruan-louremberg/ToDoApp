export type Status = "Pending" | "InProgress" | "Completed";
export type Priority = "Low" | "Medium" | "High";

export interface Task {

    id: string;
    title: string;
    description: string | null;
    status: Status;
    priority: Priority;
    dueDate: string | null;
    categoryId: string | null;
    createdAt: string;
    updatedAt: string | null;
    completedAt: string | null;

}
