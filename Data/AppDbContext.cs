using dotnet_api_tutorial.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api_tutorial.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<User> Users {get; set;}
}
