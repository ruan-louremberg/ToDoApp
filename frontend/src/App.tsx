import { TaskListPage } from "./pages/TaskListPage";
import { SummaryPanel } from "./features/tasks/summary/SummaryPanel";
export function App() {

  return (

    <div className="app-container">
      {
        <div className="header">
          <p>ToDoApp</p>
        </div>
      }
      <SummaryPanel />
      <TaskListPage />
    </div>
    
  );

}