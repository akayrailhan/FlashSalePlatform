using MediatR;
using TicketAPI.DTOs;

namespace TicketAPI.Queries
{
    public record GetAllFlightsQuery : IRequest<List<FlightDto>>;
}
