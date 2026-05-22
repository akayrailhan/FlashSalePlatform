using System.Security.Cryptography;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using TicketAPI.Commands;
using TicketAPI.Data;
using TicketAPI.Exceptions;
using TicketAPI.Models;

namespace TicketAPI.Handlers
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, string>
    {
        private static readonly char[] PnrAlphabet =
            "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

        private readonly AppDbContext _context;
        private readonly IConnectionMultiplexer _redis;

        public CreateBookingCommandHandler(AppDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis;
        }

        public async Task<string> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var lockKey = $"flight_lock_{request.FlightId}";
            var lockValue = Guid.NewGuid().ToString("N");
            var database = _redis.GetDatabase();

            var lockAcquired = await database.LockTakeAsync(
                lockKey,
                lockValue,
                TimeSpan.FromSeconds(5));

            if (!lockAcquired)
            {
                throw new ConcurrencyException(
                    "Şu anda başka bir kullanıcı bu uçuş için işlem yapıyor, lütfen tekrar deneyin.");
            }

            try
            {
                var flight = await _context.Flights
                    .FirstOrDefaultAsync(f => f.Id == request.FlightId, cancellationToken);

                if (flight is null)
                {
                    throw new NotFoundException("Uçuş bulunamadı.");
                }

                if (flight.AvailableSeats <= 0)
                {
                    throw new InvalidOperationException("Bu uçuş için tüm koltuklar doludur.");
                }

                var pnrCode = GeneratePnrCode();

                var booking = new Booking
                {
                    FlightId = request.FlightId,
                    UserId = request.UserId,
                    PnrCode = pnrCode,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);

                flight.AvailableSeats -= 1;
                await _context.SaveChangesAsync(cancellationToken);

                return pnrCode;
            }
            finally
            {
                await database.LockReleaseAsync(lockKey, lockValue);
            }
        }

        private static string GeneratePnrCode()
        {
            var suffixLength = 5;
            var buffer = new char[suffixLength];

            for (var i = 0; i < suffixLength; i += 1)
            {
                var index = RandomNumberGenerator.GetInt32(PnrAlphabet.Length);
                buffer[i] = PnrAlphabet[index];
            }

            return $"GLJ-{new string(buffer)}";
        }
    }
}
