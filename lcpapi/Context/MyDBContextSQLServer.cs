using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContextSQLServer : MyDBContext
{   
    private readonly IConfiguration _config;

    public MyDBContextSQLServer(DbContextOptions<MyDBContextSQLServer> options, IConfiguration config, ILogger<MyDBContext> logger) : base(options, config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("SQLServer")!);
        }

        base.OnConfiguring(optionsBuilder);
    }
}