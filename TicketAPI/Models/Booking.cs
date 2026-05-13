namespace TicketAPI.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string PnrCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
