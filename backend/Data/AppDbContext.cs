
using Microsoft.EntityFrameworkCore;
using TasksAPI.Models;

namespace TasksAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Todo> Todos { get; set; }

}
    
