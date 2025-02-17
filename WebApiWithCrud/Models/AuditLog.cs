namespace WebApiWithCrud.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }  
        public string UserEmail { get; } = "John.Doe@gmail.com";
        public required string EntityName { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public required string Changes { get; set; }
    }

}
