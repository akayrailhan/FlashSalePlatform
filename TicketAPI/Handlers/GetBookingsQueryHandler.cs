using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.DTOs;
using TicketAPI.Queries;

namespace TicketAPI.Handlers
{
    public class GetBookingsQueryHandler : IRequestHandler<GetBookingsQuery, List<BookingDto>>
    {
        private readonly AppDbContext _context;

        public GetBookingsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookingDto>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Where(booking => booking.UserId == request.UserId)
                .OrderByDescending(booking => booking.CreatedAt)
                .Select(booking => new BookingDto
                {
                    PnrCode = booking.PnrCode,
                    FlightId = booking.FlightId,
                    CreatedAt = booking.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}