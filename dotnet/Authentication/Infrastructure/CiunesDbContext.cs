using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure;

public class CiunexDbContext : DbContext
{
    public CiunexDbContext(DbContextOptions<CiunexDbContext> options)
        : base(options)
    { }
    
    public DbSet<User>? Users { get; set; }
}