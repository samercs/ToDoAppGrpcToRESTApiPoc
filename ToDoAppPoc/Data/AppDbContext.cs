using Microsoft.EntityFrameworkCore;
using ToDoAppPoc.Models;

namespace ToDoAppPoc.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<ToDoItem> Items => Set<ToDoItem>();
}