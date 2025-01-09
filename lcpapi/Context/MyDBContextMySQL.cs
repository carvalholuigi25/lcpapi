using Microsoft.EntityFrameworkCore;

namespace lcpapi.Context;

public class MyDBContextMySQL : MyDBContext
{   
    private readonly IConfiguration _config;

    public MyDBContextMySQL(DbContextOptions<MyDBContextMySQL> options, IConfiguration config) : base(options, config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured) {
            optionsBuilder.UseMySql(_config.GetConnectionString("MySQL")!, new MySqlServerVersion(new Version()));
        }

        base.OnConfiguring(optionsBuilder);
    }
}