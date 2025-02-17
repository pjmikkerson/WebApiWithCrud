using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using WebApiWithCrud.Endpoints;
using WebApiWithCrud.Persistence;
using WebApiWithCrud.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
try
{

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



    builder.Services.AddOpenApi();
    builder.Services.AddDbContext<MovieDbContext>(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString);
    });
    builder.Services.AddTransient<IDummyService, DummyService>();
    builder.Services.AddTransient<IMovieService, MovieService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    await using (var serviceScope = app.Services.CreateAsyncScope())
    await using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<MovieDbContext>())
    {
        await dbContext.Database.EnsureCreatedAsync();
    }

    app.UseHttpsRedirection();

    app.MapGet("/", (IDummyService svc) => svc.DoSomething());

    app.MapMovieEndpoints();

    app.Run();

}
catch (Exception ex)
{

    Log.Error(ex, "Stopped program because of exception");

}

finally
{

    Log.CloseAndFlush();
}
