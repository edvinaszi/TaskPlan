using System;
using Microsoft.Data.Sqlite;

namespace TaskPlan;

class Program
{
    static void Main(string[] args)
    {
        string dbPath = "Data Source=tasks.db"; 

        DatabaseHelper dbHelper = new DatabaseHelper(dbPath);
        dbHelper.CreateDatabase();


        var taskManager = new TaskManager(dbHelper);
        MainMenu(taskManager, dbHelper);
    }

    static void MainMenu(TaskManager taskManager, DatabaseHelper dbHelper)
    {
        bool AtMenu = true;

        while (AtMenu)
        {
            Console.Clear();

            taskManager.DisplayTodaysTasks();

            Console.WriteLine("1. All tasks");
            Console.WriteLine("2. Completed tasks");
            Console.WriteLine("3. Overdue tasks");
            Console.WriteLine("4. School");
            Console.WriteLine("5. Freetime");
            Console.WriteLine("6. Home");
            Console.WriteLine("7. Task manager");
            Console.WriteLine("8. Exit");

            var choice = Console.ReadKey().KeyChar;

            switch (choice)
            {
                case '1':
                    Console.Clear();
                    taskManager.ShowAllTasks();
                    BackToMenu();
                    break;
                case '2':
                    Console.Clear();
                    taskManager.DisplayTasks(taskManager.CompletedTasks());
                    BackToMenu();
                    break;
                case '3':
                    Console.Clear();
                    taskManager.OverdueTasks();
                    BackToMenu();
                    break;
                case '4':
                    Console.Clear();
                    taskManager.DisplayTasksByType(TaskType.School);
                    BackToMenu();
                    break;
                case '5':
                    Console.Clear();
                    taskManager.DisplayTasksByType(TaskType.Freetime);
                    BackToMenu();
                    break;
                case '6':
                    Console.Clear();
                    taskManager.DisplayTasksByType(TaskType.Home);
                    BackToMenu();
                    break;
                case '7':
                    Console.Clear();
                    TaskManagerMenu(taskManager, dbHelper);
                    break;
                case '8':
                    Console.Clear();
                    AtMenu = false;
                    Console.WriteLine("Bye.");
                    break;
                default:
                    break;
            }
        }


    }

    static void TaskManagerMenu(TaskManager taskManager, DatabaseHelper dbHelper)
    {
        bool AtMenu = true;

        while (AtMenu)
        {
            Console.Clear();

            Console.WriteLine("1. Add task");
            Console.WriteLine("2. Edit task");
            Console.WriteLine("3. Delete task");
            Console.WriteLine("4. Mark task completed");
            Console.WriteLine("5. Extend time");
            Console.WriteLine("6. Back to main menu");

            var choice = Console.ReadKey().KeyChar;

            switch (choice)
            {
                case '1':
                    Console.Clear();
                    taskManager.AddTask();
                    BackToMenu();
                    break;
                case '2':
                    Console.Clear();
                    taskManager.EditTask();
                    break;
                case '3':
                    Console.Clear();
                    taskManager.DeleteTask();
                    BackToMenu();
                    break;
                case '4':
                    Console.Clear();
                    taskManager.CompleteTask();
                    BackToMenu();
                    break;
                case '5':
                    Console.Clear();
                    taskManager.ExtendTask();
                    BackToMenu();
                    break;
                case '6':
                    Console.Clear();
                    AtMenu = false;
                    break;
                default:
                    break;
            }
        }

    }

    static void BackToMenu()
    {
        Console.WriteLine("\nPress any key to go back to the main menu.");
        Console.ReadKey();
    }


}
