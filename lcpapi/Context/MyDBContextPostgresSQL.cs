using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContextPostgresSQL : MyDBContext
{   
    private readonly IConfiguration _config;

    public MyDBContextPostgresSQL(DbContextOptions<MyDBContextPostgresSQL> options, IConfiguration config) : base(options, config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            optionsBuilder.UseNpgsql(_config.GetConnectionString("PostgresSQL")!);
        }

        base.OnConfiguring(optionsBuilder);
    }
}