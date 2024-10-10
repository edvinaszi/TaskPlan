using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskPlan
{
    public class TaskManager
    {
        private DatabaseHelper dbHelper;

        public TaskManager(DatabaseHelper dbHelper)
        {
            this.dbHelper = dbHelper;
        }

        public void DisplayTasks(List<TaskPlan.Task> tasks)
        {
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].DisplayTask(i + 1); 
            }

        }

        public void ShowAllTasks()
        {
            var tasksFromDb = dbHelper.GetAllTasks();

            if (tasksFromDb.Count == 0)
            {
                Console.WriteLine("No tasks to show.\n");
                return;
            }

            DisplayTasks(tasksFromDb);
        }

        public void DisplayTasksByType(TaskType type)
        {
            var tasksByType = dbHelper.GetAllTasks().Where(task => task.Type == type).ToList();

            if (tasksByType.Count == 0)
            {
                Console.WriteLine($"No tasks found for type: {type}\n");
                return;
            }

            Console.WriteLine($"\nTasks of type: {type}");
            DisplayTasks(tasksByType); 
        }

        public void DisplayTodaysTasks()
        {
            var tasksForToday = dbHelper.GetAllTasks().Where(task => task.Date == DateOnly.FromDateTime(DateTime.Now) && !task.Completed);

            if (tasksForToday.Any())
            {
                Console.WriteLine("Tasks that need to be done today:\n");

                foreach (var task in tasksForToday)
                {
                    if (!task.Completed)
                    {
                        Console.WriteLine(task.Name.ToUpper());
                    }
                }
                Console.WriteLine("\n\n");
            }
        }

        public void AddTask()
        {
            string name = Name();
            Console.Clear();
            string todo = Description();
            Console.Clear();
            TaskType type = Type();
            Console.Clear();
            DateOnly date = Date();  
            Console.Clear();

            Task task = new Task(name, todo, date, type);  

            int taskId = dbHelper.InsertTask(task);

            task.Id = taskId;

            Console.Clear();
            Console.WriteLine($"{task.Name} was added to {task.Type} tasks.");
        }

        public void DeleteTask()
        {
            ShowAllTasks();
            int taskNr = GetTaskNumber("delete");
            var tasks = dbHelper.GetAllTasks();

            if (taskNr > 0 && taskNr <= tasks.Count)
            {
                Task selectedTask = tasks[taskNr - 1];
                dbHelper.DeleteTask(selectedTask.Id);
                Console.Clear();
                Console.WriteLine($"{selectedTask.Name} was removed from your tasks.\n");
            }
            else
            {
                Console.WriteLine("Invalid task number. Please try again.");
            }
        }



        public void CompleteTask()
        {
            var incompleteTasks = dbHelper.GetAllTasks().Where(task => !task.Completed).ToList();

            if (incompleteTasks.Count == 0)
            {
                Console.WriteLine("No incomplete tasks available.");
                return;
            }

            DisplayTasks(incompleteTasks); 
            int taskNr = GetTaskNumber("complete"); 

            if (taskNr > 0 && taskNr <= incompleteTasks.Count)
            {
                Task selectedTask = incompleteTasks[taskNr - 1]; 

                selectedTask.MarkAsCompleted(); 
                dbHelper.UpdateTask(selectedTask); 

                Console.Clear();
                Console.WriteLine($"Task '{selectedTask.Name}' marked as completed.");
            }
            else
            {
                Console.WriteLine("Invalid task number. Please try again.");
            }
        }

        private int GetTaskNumber(string action)
        {
            int taskNr = -1;
            while (true)
            {
                Console.Write($"\nEnter the number of task you want to {action}: ");
                var choice = Console.ReadLine();
                bool choiceParse = int.TryParse(choice, out taskNr);

                if (!choiceParse || taskNr <= 0)
                {
                    Console.WriteLine("Enter a valid number.");
                }
                else
                {
                    break;
                }
            }
            return taskNr;
        }

        public void ExtendTask()
        {
            ShowAllTasks();
            int taskNr = GetTaskNumber("extend");
            var tasks = dbHelper.GetAllTasks();
            Task selectedTask = tasks[taskNr - 1];

            Console.Clear();
            DateOnly newDate = Date();
            selectedTask.UpdateDate(newDate);

            dbHelper.UpdateTask(selectedTask);
            Console.Clear();
            Console.WriteLine($"New date for {selectedTask.Name.ToUpper()} was changed successfully.");
        }

        public void OverdueTasks()
        {
            var tasks = dbHelper.GetAllTasks();
            int i = 1;
            bool hasOverdueTasks = false;

            foreach (var task in tasks)
            {
                if (task.Date < DateOnly.FromDateTime(DateTime.Now) && !task.Completed)
                {
                    task.DisplayTask(i++);
                    hasOverdueTasks = true;
                }
            }

            if (!hasOverdueTasks)
            {
                Console.WriteLine("No overdue tasks to show.");
            }
        }

        public void EditTask()
        {
            ShowAllTasks();
            int taskNr = GetTaskNumber("edit");
            var tasks = dbHelper.GetAllTasks();
            Task selectedTask = tasks[taskNr - 1];

            EditMenu(selectedTask);
            dbHelper.UpdateTask(selectedTask);
        }

        public void EditMenu(Task selectedTask)
        {
            bool atMenu = true;

            while (atMenu)
            {
                Console.Clear();

                Console.WriteLine("What would you like to edit for the selected task?\n");
                Console.WriteLine("1. Name");
                Console.WriteLine("2. Description");
                Console.WriteLine("3. Deadline");
                Console.WriteLine("4. Type");
                Console.WriteLine("5. Exit");

                var choice = Console.ReadKey().KeyChar;

                switch (choice)
                {
                    case '1':
                        Console.Clear();
                        string name = Name();
                        selectedTask.UpdateName(name);
                        break;
                    case '2':
                        Console.Clear();
                        string todo = Description();
                        selectedTask.UpdateTodo(todo);
                        break;
                    case '3':
                        Console.Clear();
                        DateOnly date = Date();
                        selectedTask.UpdateDate(date);
                        break;
                    case '4':
                        Console.Clear();
                        TaskType type = Type();
                        selectedTask.UpdateType(type);
                        break;
                    case '5':
                        atMenu = false;
                        break;
                    default:
                        break;
                }
                Console.Clear();
            }
        }

        public DateOnly Date()
        {
            DateOnly date;

            while (true)
            {
                Console.Write("Enter the deadline date (yyyy-mm-dd): ");
                string dateToParse = Console.ReadLine();
                var parsing = DateOnly.TryParse(dateToParse, out date);

                if (string.IsNullOrEmpty(dateToParse))
                {
                    Console.WriteLine("Date cannot be empty");
                }
                else if (!parsing)
                {
                    Console.WriteLine("Enter a valid date in the format shown.");
                }
                else
                {
                    break;
                }
            }

            return date;
        }

        public string Name()
        {
            string name;

            while (true)
            {
                Console.Write("Enter the name of the task: ");
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name cannot be empty");
                }
                else
                {
                    break;
                }
            }

            return name;
        }

        public string Description()
        {
            string todo;

            while (true)
            {
                Console.WriteLine("What needs to be done?");
                todo = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(todo))
                {
                    Console.WriteLine("Description cannot be empty");
                }
                else
                {
                    break;
                }
            }

            return todo;
        }

        public TaskType Type()
        {
            TaskType type;

            while (true)
            {
                Console.Write("What type of task is it (home, freetime, school): ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty");
                    continue;
                }

                if (Enum.TryParse<TaskType>(input, true, out type))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid task type");
                }
            }
            return type;
        }

        public List<Task> CompletedTasks()
        {
            var allTasks = dbHelper.GetAllTasks();

            var completedTasks = allTasks.Where(task => task.Completed).ToList();

            return completedTasks;
        }
    }
}