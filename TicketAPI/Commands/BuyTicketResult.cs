namespace TicketAPI.Commands
{
    public class BuyTicketResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public Guid? OrderId { get; init; }
        public int StatusCode { get; init; }
    }
}
