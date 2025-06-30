namespace StockApp.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public string Action { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}

