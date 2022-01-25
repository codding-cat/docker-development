using Authentication.Models;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure;

public class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

}