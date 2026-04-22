namespace TicketAPI.DTOs
{
    public class FlightDto
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public int AvailableSeats { get; set; }
    }
}