namespace TasksAPI.Models;

    public class Todo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public bool Completed { get; set; }

    }

