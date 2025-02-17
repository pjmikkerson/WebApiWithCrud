using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using WebApiWithCrud.Models;

namespace WebApiWithCrud.Persistence
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("app");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Your existing seeding configuration (if needed)
            string filePath = @"C:\Dev\WebApiWithCrud\WebApiWithCrud\Data\movies.txt";

            optionsBuilder
                .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        return;
                    }

                    var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                            continue;

                        var parts = line.Split(',');
                        if (parts.Length < 4)
                            continue;

                        var title = parts[0].Trim();
                        var genre = parts[1].Trim();

                        if (!DateTime.TryParse(parts[2].Trim(), out var releaseDate))
                            continue;

                        if (!int.TryParse(parts[3].Trim(), out var rating))
                            continue;

                        bool movieExists = await context.Set<Movie>().AnyAsync(m => m.Title == title, cancellationToken);
                        if (!movieExists)
                        {
                            var movie = Movie.Create(title, genre, new DateTimeOffset(releaseDate, TimeSpan.Zero), rating);
                            await context.Set<Movie>().AddAsync(movie, cancellationToken);
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                })
                .UseSeeding((context, _) =>
                {
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"File not found: {filePath}");
                        return;
                    }

                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                            continue;

                        var parts = line.Split(',');
                        if (parts.Length < 4)
                            continue;

                        var title = parts[0].Trim();
                        var genre = parts[1].Trim();

                        if (!DateTime.TryParse(parts[2].Trim(), out var releaseDate))
                            continue;

                        if (!int.TryParse(parts[3].Trim(), out var rating))
                            continue;

                        bool movieExists = context.Set<Movie>().Any(m => m.Title == title);
                        if (!movieExists)
                        {
                            var movie = Movie.Create(title, genre, new DateTimeOffset(releaseDate, TimeSpan.Zero), rating);
                            context.Set<Movie>().Add(movie);
                        }
                    }

                    context.SaveChanges();
                });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Retrieve all tracked entries that are Added, Modified, or Deleted
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in modifiedEntities)
            {
                // Skip AuditLog itself to prevent recursive logging
                if (entry.Entity is AuditLog)
                    continue;

                var auditLog = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    Action = entry.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Changes = GetChanges(entry)
                };

                AuditLogs.Add(auditLog);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static string GetChanges(EntityEntry entry)
        {
            var changes = new StringBuilder();

            // Loop through each property to detect changes
            foreach (var property in entry.OriginalValues.Properties)
            {
                var originalValue = entry.OriginalValues[property];
                var currentValue = entry.CurrentValues[property];

                // For added entities, log the new values.
                if (entry.State == EntityState.Added)
                {
                    changes.AppendLine($"{property.Name}: set to '{currentValue}'");
                }
                // For deleted entities, log the removed values.
                else if (entry.State == EntityState.Deleted)
                {
                    changes.AppendLine($"{property.Name}: removed (was '{originalValue}')");
                }
                // For modified entities, log the differences.
                else if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: changed from '{originalValue}' to '{currentValue}'");
                }
            }

            return changes.ToString();
        }
    }
}
