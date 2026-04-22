namespace TicketAPI.DTOs
{
    public class BuyTicketRequestDto
    {
        public string UserEmail { get; set; } = string.Empty;
        public int SeatCount { get; set; }
    }
}