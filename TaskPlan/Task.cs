using System;

namespace TaskPlan
{
    public class Task
    {
        public int Id { get; set; } 
        public string Name { get; private set; }
        public string ToDo { get; private set; }
        public DateOnly Date { get; private set; }
        public bool Completed { get; private set; }
        public DateTime? TaskDone { get; private set; }
        public TaskType Type { get; private set; }

        public Task(string name, string todo, DateOnly date, TaskType type, bool completed = false)
        {
            Name = name;
            ToDo = todo;
            Date = date;
            Completed = completed;
            Type = type;
            TaskDone = completed ? DateTime.Now : (DateTime?)null;
        }

        public Task(int id, string name, string todo, DateOnly date, TaskType type, bool completed = false)
        {
            Id = id;
            Name = name;
            ToDo = todo;
            Date = date;
            Type = type;
            Completed = completed;
            TaskDone = completed ? DateTime.Now : (DateTime?)null;
        }

        public void DisplayTask(int i)
        {
            Console.WriteLine($"{i}. {Name.ToUpper()}\n{ToDo}\nDeadline: {Date}\nType: {Type}");
            Console.Write("Completed: ");

            if (Completed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("V");
                Console.WriteLine($"Completed on: {TaskDone}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("X");
            }

            Console.ResetColor();
        }

        public void MarkAsCompleted()
        {
            Completed = true;
            TaskDone = DateTime.Now;
        }

        public void UpdateDate(DateOnly newDate)
        {
            Date = newDate;
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void UpdateTodo(string todo)
        {
            ToDo = todo;
        }

        public void UpdateType(TaskType type)
        {
            Type = type;
        }
    }
}