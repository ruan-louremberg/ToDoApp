import type { Priority } from "./task";

export interface TaskFormData {
  title: string;
  description: string;
  priority: Priority;
  dueDate: string;
  categoryId: string;
}
