namespace TicketAPI.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int SeatCount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
    }
}