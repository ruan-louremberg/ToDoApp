import { TaskListPage } from "./pages/TaskListPage";

export function App() {

  return (

    <div className="app-container">
      {
        <div className="header">
          <p>ToDoApp</p>
        </div>
      }
      <TaskListPage />
    </div>
    
  );

}