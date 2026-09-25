import { TaskListPage } from "./pages/TaskListPage";
import { TaskSummaryPage } from "./pages/TaskSummaryPage";

export function App() {

  return (

    <div className="app-container">
      {
        <div className="header">
          <p>ToDoApp</p>
        </div>
      }
      <TaskSummaryPage />
      <TaskListPage />
    </div>
    
  );

}