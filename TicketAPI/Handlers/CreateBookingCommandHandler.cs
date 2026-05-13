using System.Security.Cryptography;
using MediatR;
using TicketAPI.Commands;
using TicketAPI.Data;
using TicketAPI.Models;

namespace TicketAPI.Handlers
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, string>
    {
        private static readonly char[] PnrAlphabet =
            "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();

        private readonly AppDbContext _context;

        public CreateBookingCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var pnrCode = GeneratePnrCode(); // PNR kodu uretimi

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
