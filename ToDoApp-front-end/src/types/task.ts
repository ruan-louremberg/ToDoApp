export type Status = "Pending" | "InProgress" | "Completed";
export type Priority = "Low" | "Medium" | "High";

export interface Task {

    id: string;
    title: string;
    description: string | null;
    status: Status;
    priority: Priority;
    dueDate: string | null;
    category: string | null;
    completedAt: string | null;

}

export interface CreateTaskData {

    title: string;
    description?: string;
    priority: number;
    dueDate?: string | null;
    
}   