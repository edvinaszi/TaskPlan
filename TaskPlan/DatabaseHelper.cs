using Microsoft.Data.Sqlite;
using TaskPlan;

public class DatabaseHelper
{
    private string _connectionString;

    public DatabaseHelper(string dbPath)
    {
        _connectionString = dbPath;
        Console.WriteLine($"Using database at: {dbPath}");
    }

    public void CreateDatabase()
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            CreateTable(connection);
        }
    }

    private void CreateTable(SqliteConnection connection)
    {
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText =
        @"
    CREATE TABLE IF NOT EXISTS Tasks (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Title TEXT NOT NULL,
        Description TEXT,
        Date TEXT NOT NULL,  -- Make sure 'Date' is added
        Type TEXT NOT NULL,
        IsCompleted INTEGER NOT NULL DEFAULT 0
    );
    ";
        createTableCmd.ExecuteNonQuery();
    }

    public int InsertTask(TaskPlan.Task task)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText =
            @"
            INSERT INTO Tasks (Title, Description, Date, Type, IsCompleted)
            VALUES ($title, $description, $date, $type, $isCompleted);
            
            SELECT last_insert_rowid();  -- Retrieve the auto-incremented ID
        ";
            insertCmd.Parameters.AddWithValue("$title", task.Name);
            insertCmd.Parameters.AddWithValue("$description", task.ToDo);
            insertCmd.Parameters.AddWithValue("$date", task.Date.ToString("yyyy-MM-dd"));
            insertCmd.Parameters.AddWithValue("$type", task.Type.ToString());
            insertCmd.Parameters.AddWithValue("$isCompleted", task.Completed ? 1 : 0);

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }
    }

    public List<TaskPlan.Task> GetAllTasks()
    {
        var tasks = new List<TaskPlan.Task>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT * FROM Tasks;";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        var id = reader.GetInt32(0);
                        var title = reader.GetString(1);
                        var description = reader.GetString(2);
                        var dateStr = reader.GetString(3);
                        var typeStr = reader.GetString(4);
                        var isCompleted = reader.GetInt32(5) == 1;

                        var task = new TaskPlan.Task(
                            id,
                            title,
                            description,
                            DateOnly.Parse(dateStr),
                            Enum.Parse<TaskType>(typeStr), 
                            isCompleted
                        );
                        tasks.Add(task);
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine($"Date format error: {ex.Message}");
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine($"Column out of range error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"General error: {ex.Message}");
                    }
                }
            }
        }

        return tasks;
    }

    public void UpdateTask(TaskPlan.Task task)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var updateCmd = connection.CreateCommand();
            updateCmd.CommandText =
            @"
        UPDATE Tasks 
        SET Title = $title, Description = $description, Date = $date, Type = $type, IsCompleted = $isCompleted 
        WHERE Id = $id;
        ";
            updateCmd.Parameters.AddWithValue("$title", task.Name);
            updateCmd.Parameters.AddWithValue("$description", task.ToDo);
            updateCmd.Parameters.AddWithValue("$date", task.Date.ToString("yyyy-MM-dd"));
            updateCmd.Parameters.AddWithValue("$type", task.Type.ToString());
            updateCmd.Parameters.AddWithValue("$isCompleted", task.Completed ? 1 : 0);
            updateCmd.Parameters.AddWithValue("$id", task.Id);
            updateCmd.ExecuteNonQuery();
        }
    }

    public void DeleteTask(int taskId)
    {
        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Tasks WHERE Id = $id;";
            deleteCmd.Parameters.AddWithValue("$id", taskId);
            deleteCmd.ExecuteNonQuery();
        }
    }

    public List<TaskPlan.Task> GetTasksByType(TaskType type)
    {
        var tasks = new List<TaskPlan.Task>();

        using (var connection = new SqliteConnection(_connectionString))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tasks WHERE Type = $type";
            command.Parameters.AddWithValue("$type", type.ToString());

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                        var task = new TaskPlan.Task(
                        reader.GetString(1), 
                        reader.GetString(2), 
                        DateOnly.Parse(reader.GetString(3)),
                        Enum.Parse<TaskType>(reader.GetString(4)), 
                        reader.GetInt32(5) == 1 
                    );
                    tasks.Add(task);
                }
            }
        }

        return tasks;
    }



}