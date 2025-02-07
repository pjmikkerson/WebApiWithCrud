using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApiWithCrud.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MovieDbContext>
{
    public MovieDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MovieDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dotnetHero;Username=postgres;Password=postgres");

        return new MovieDbContext(optionsBuilder.Options);
    }
}