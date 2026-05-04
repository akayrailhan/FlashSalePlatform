using MediatR;
using TicketAPI.Models;

namespace TicketAPI.Commands
{
    public record CreateFlightCommand(Flight Flight) : IRequest<Flight>;
}
