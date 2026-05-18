using MediatR;
using TicketAPI.DTOs;

namespace TicketAPI.Queries
{
    public record GetBookingsQuery(string UserId) : IRequest<List<BookingDto>>;
}