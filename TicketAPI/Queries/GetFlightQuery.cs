using MediatR;
using TicketAPI.DTOs;

namespace TicketAPI.Queries
{
    public record GetFlightQuery(Guid Id) : IRequest<FlightDto?>;
}
