using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiWithCrud.Models;


namespace WebApiWithCrud.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Define table name
            builder.ToTable("AuditLogs");

            // Set primary key
            builder.HasKey(a => a.Id);

            // Configure properties
            builder.Property(a => a.UserEmail)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.EntityName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(a => a.Action)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(a => a.Timestamp)
                   .IsRequired();

            builder.Property(a => a.Changes)
                   .IsRequired();
        }
    }
}
