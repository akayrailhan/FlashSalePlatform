using MediatR;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.DTOs;
using TicketAPI.Queries;

namespace TicketAPI.Handlers
{
    public class GetAllFlightsQueryHandler : IRequestHandler<GetAllFlightsQuery, List<FlightDto>>
    {
        private readonly AppDbContext _context;

        public GetAllFlightsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightDto>> Handle(GetAllFlightsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Flights
                .AsNoTracking()
                .Select(flight => new FlightDto
                {
                    Id = flight.Id,
                    FlightNumber = flight.FlightNumber,
                    Origin = flight.Origin,
                    Destination = flight.Destination,
                    DepartureTime = flight.DepartureTime,
                    BasePrice = flight.BasePrice,
                    AvailableSeats = flight.AvailableSeats
                })
                .ToListAsync(cancellationToken);
        }
    }
}
