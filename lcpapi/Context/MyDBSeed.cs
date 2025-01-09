using lcpapi.Models;
using lcpapi.Models.Enums;
using Microsoft.EntityFrameworkCore;

public class MyDBSeed
{
    private readonly ModelBuilder _modelBuilder;
    
    public MyDBSeed(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }

    public void Seed(bool isseed = false)
    {
        if(isseed) {
            _modelBuilder.Entity<User>().HasData(
                new User() { 
                    Id = 1, 
                    Username = "admin", 
                    DisplayName = "Luis Carvalho", 
                    Password = BCrypt.Net.BCrypt.HashPassword("1234", 10, false), 
                    Role = EUsersRoles.admin 
                }
            );
        }
    }
}
