using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // می‌توانید Seed اولیه هم اینجا اضافه کنید اگر لازم بود.
    }
}
