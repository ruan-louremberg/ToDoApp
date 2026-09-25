export interface TaskSummaryResponse {
  totalTasks: number;
  status: {
    pending: number;
    inProgress: number;
    completed: number;
  };
  delays: {
    overdue: number;
  };
}