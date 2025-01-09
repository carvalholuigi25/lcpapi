using lcpapi.Models;
using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContext : DbContext
{
    private readonly IConfiguration _config;

    public MyDBContext(DbContextOptions options, IConfiguration config)
        : base(options)
    {
        _config = config;
    }

    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            var defdbm = _config.GetSection("DefDBMode").Value ?? "MemoryDB";

            if(defdbm == "MemoryDB") {
                optionsBuilder.UseInMemoryDatabase("DBContext");
            }
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        new MyDBSeed(modelBuilder).Seed(false);
    }
}