using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContextSQLite : MyDBContext
{   
    private readonly IConfiguration _config;

    public MyDBContextSQLite(DbContextOptions<MyDBContextSQLite> options, IConfiguration config) : base(options, config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlite(_config.GetConnectionString("SQLite")!);
        }

        base.OnConfiguring(optionsBuilder);
    }
}