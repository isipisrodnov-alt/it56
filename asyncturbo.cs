
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace TaskManager
{
    public enum Priority { Low, Medium, High }
    public enum Status { NotStarted, InProgress, Completed }

    public class User { public string Username { get; set; } public string PasswordHash { get; set; } }
    public class TaskItem { public string TaskId { get; set; } public string Title { get; set; } public string Description { get; set; } public Priority Priority { get; set; } public Status Status { get; set; } public string OwnerUsername { get; set; } }

    class Program
    {
        private const string UsersFile = "users.txt";
        private const string TasksFile = "tasks.txt";

        private static async Task<string> GenerateId(string file)
        {
            string id;
            do
            {
                id = Guid.NewGuid().ToString();
                try
                {
                    using (var reader = new StreamReader(file, Encoding.UTF8))
                    {
                        string content = await reader.ReadToEndAsync();
                        if (!content.Contains($"\"TaskId\":\"{id}\"")) return id;
                    }
                }
                catch { }
            } while (true);
        }

        private static async Task<List<User>> LoadUsers()
        {
            try
            {
                using (var reader = new StreamReader(UsersFile, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
            }
            catch { return new List<User>(); }
        }

        private static async Task SaveUsers(List<User> users)
        {
            string json = JsonSerializer.Serialize(users);
            using (var writer = new StreamWriter(UsersFile, false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }

        private static async Task<bool> Register(string u, string p)
        {
            var users = await LoadUsers();
            if (users.Any(x => x.Username == u)) { Console.WriteLine("Exists"); return false; }
            users.Add(new User { Username = u, PasswordHash = Hash(p) });
            await SaveUsers(users);
            Console.WriteLine("OK");
            return true;
        }

        private static async Task<User> Auth(string u, string p)
        {
            var users = await LoadUsers();
            var user = users.FirstOrDefault(x => x.Username == u && x.PasswordHash == Hash(p));
            if (user != null) Console.WriteLine("OK");
            return user;
        }

        private static string Hash(string p)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(p)));
            }
        }

        private static async Task<List<TaskItem>> LoadTasks()
        {
            try
            {
                using (var reader = new StreamReader(TasksFile, Encoding.UTF8))
                {
                    string json = await reader.ReadToEndAsync();
                    return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
                }
            }
            catch { return new List<TaskItem>(); }
        }

        private static async Task SaveTasks(List<TaskItem> tasks)
        {
            string json = JsonSerializer.Serialize(tasks);
            using (var writer = new StreamWriter(TasksFile, false, Encoding.UTF8))
            {
                await writer.WriteAsync(json);
            }
        }

        private static async Task Create(User user)
        {
            Console.Write("Title: "); string t = Console.ReadLine();
            Console.Write("Desc: "); string d = Console.ReadLine();
            Console.Write("Priority (Low,Medium,High):");
            if (!Enum.TryParse(Console.ReadLine(), true, out Priority pr)) { Console.WriteLine("Invalid Priority"); return; }
            Console.Write("Status (NotStarted,InProgress,Completed): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Status st)) { Console.WriteLine("Invalid Status"); return; }

            var tasks = await LoadTasks();
            tasks.Add(new TaskItem { TaskId = await GenerateId(TasksFile), Title = t, Description = d, Priority = pr, Status = st, OwnerUsername = user.Username });
            await SaveTasks(tasks);
            Console.WriteLine("OK");
        }

        private static async Task List(User user)
        {
            var tasks = await LoadTasks();
            foreach (var task in tasks.Where(x => x.OwnerUsername == user.Username))
                Console.WriteLine($"ID:{task.TaskId} - {task.Title} - {task.Priority} - {task.Status}");
        }

        private static async Task Edit(User user)
        {
            Console.Write("Task ID: "); string id = Console.ReadLine();
            var tasks = await LoadTasks();
            var task = tasks.FirstOrDefault(x => x.TaskId == id && x.OwnerUsername == user.Username);
            if (task == null) { Console.WriteLine("Not found"); return; }

            Console.Write("New title: "); task.Title = Console.ReadLine();
            Console.Write("New Desc: "); task.Description = Console.ReadLine();

            Console.Write("New Priority (Low,Medium,High): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Priority pr)) { Console.WriteLine("Invalid Priority"); return; }
            task.Priority = pr;

            Console.Write("New Status (NotStarted,InProgress,Completed): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out Status st)) { Console.WriteLine("Invalid Status"); return; }
            task.Status = st;

            await SaveTasks(tasks);
            Console.WriteLine("OK");
        }

        private static async Task Delete(User user)
        {
            Console.Write("Task ID: "); string id = Console.ReadLine();
            var tasks = await LoadTasks();
            tasks.RemoveAll(x => x.TaskId == id && x.OwnerUsername == user.Username);
            await SaveTasks(tasks);
            Console.WriteLine("OK");
        }

        static async Task Main(string[] args)
        {
            User user = null;
            while (true)
            {
                Console.WriteLine("\n1. Login\n2. Register\n3. Exit");
                string choice = Console.ReadLine();
                if (choice == "1") { Console.Write("User: "); string u = Console.ReadLine(); Console.Write("Pass: "); string p = Console.ReadLine(); user = await Auth(u, p); }
                else if (choice == "2") { Console.Write("User: "); string u = Console.ReadLine(); Console.Write("Pass: "); string p = Console.ReadLine(); await Register(u, p); }
                else if (choice == "3") { break; }
                else Console.WriteLine("Invalid");
                if (user != null) break;
            }

            while (user != null)
            {
                Console.WriteLine("\n1. List\n2. Create\n3. Edit\n4. Delete\n5. Logout");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": await List(user); break;
                    case "2": await Create(user); break;
                    case "3": await Edit(user); break;
                    case "4": await Delete(user); break;
                    case "5": user = null; break;
                    default: Console.WriteLine("Invalid"); break;
                }
            }
        }
    }
}