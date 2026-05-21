using MediatR;
using TicketAPI.Commands;
using TicketAPI.Data;
using TicketAPI.Models;

namespace TicketAPI.Handlers
{
    public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, Flight>
    {
        private readonly AppDbContext _context;

        public CreateFlightCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Flight> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
        {
            if (request.Flight.AvailableSeats <= 0)
            {
                request.Flight.AvailableSeats = request.Flight.TotalCapacity;
            }

            _context.Flights.Add(request.Flight);
            await _context.SaveChangesAsync(cancellationToken);
            return request.Flight;
        }
    }
}
