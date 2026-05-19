using System.Security.Cryptography;
using MediatR;
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
        private const int LockDurationSeconds = 10;

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

            var acquired = await database.StringSetAsync(
                lockKey,
                lockValue,
                TimeSpan.FromSeconds(LockDurationSeconds),
                When.NotExists);

            if (!acquired)
            {
                throw new ConcurrencyException(
                    "Şu anda başka bir kullanıcı bu uçuş için işlem yapıyor, lütfen tekrar deneyin.");
            }

            try
            {
                var pnrCode = GeneratePnrCode();

                var booking = new Booking
                {
                    FlightId = request.FlightId,
                    UserId = request.UserId,
                    PnrCode = pnrCode,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync(cancellationToken);

                return pnrCode;
            }
            finally
            {
                const string releaseScript = @"
if redis.call('GET', KEYS[1]) == ARGV[1] then
    return redis.call('DEL', KEYS[1])
end
return 0";

                await database.ScriptEvaluateAsync(
                    releaseScript,
                    [lockKey],
                    [lockValue]);
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
