namespace TicketAPI.DTOs
{
    public class BookingDto
    {
        public string PnrCode { get; set; } = string.Empty;
        public Guid FlightId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}