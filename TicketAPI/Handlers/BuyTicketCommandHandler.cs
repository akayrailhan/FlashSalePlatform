using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TicketAPI.Commands;
using TicketAPI.Data;
using TicketAPI.Models;

namespace TicketAPI.Handlers
{
    public class BuyTicketCommandHandler : IRequestHandler<BuyTicketCommand, BuyTicketResult>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public BuyTicketCommandHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<BuyTicketResult> Handle(BuyTicketCommand request, CancellationToken cancellationToken)
        {
            var flight = await _context.Flights.FindAsync(new object?[] { request.FlightId }, cancellationToken);
            if (flight == null)
            {
                return new BuyTicketResult
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Uçuş bulunamadı."
                };
            }

            if (flight.AvailableSeats < request.SeatCount)
            {
                return new BuyTicketResult
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Yetersiz koltuk. Sadece {flight.AvailableSeats} koltuk kaldı."
                };
            }

            flight.AvailableSeats -= request.SeatCount;
            flight.Version += 1;

            var newOrder = new Order
            {
                FlightId = flight.Id,
                UserEmail = request.UserEmail,
                SeatCount = request.SeatCount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(newOrder);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _cache.RemoveAsync($"flight_{request.FlightId}", cancellationToken);

                return new BuyTicketResult
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Bilet başarıyla satın alındı!",
                    OrderId = newOrder.Id
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new BuyTicketResult
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Siz işlemi tamamlarken biletler tükendi veya başkası tarafından alındı. Lütfen tekrar deneyin."
                };
            }
        }
    }
}
