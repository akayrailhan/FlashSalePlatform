using MediatR;

namespace TicketAPI.Commands
{
    public record BuyTicketCommand(Guid FlightId, string UserEmail, int SeatCount) : IRequest<BuyTicketResult>;
}
