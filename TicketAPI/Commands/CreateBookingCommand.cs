using MediatR;

namespace TicketAPI.Commands
{
    public record CreateBookingCommand(Guid FlightId, string UserId) : IRequest<string>;
}
