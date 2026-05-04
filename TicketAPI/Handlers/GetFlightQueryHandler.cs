using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TicketAPI.Data;
using TicketAPI.DTOs;
using TicketAPI.Queries;

namespace TicketAPI.Handlers
{
    public class GetFlightQueryHandler : IRequestHandler<GetFlightQuery, FlightDto?>
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public GetFlightQueryHandler(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<FlightDto?> Handle(GetFlightQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"flight_{request.Id}";

            var cachedFlight = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedFlight))
            {
                return JsonSerializer.Deserialize<FlightDto>(cachedFlight);
            }

            var flight = await _context.Flights.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (flight == null)
            {
                return null;
            }

            var dto = new FlightDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                Origin = flight.Origin,
                Destination = flight.Destination,
                DepartureTime = flight.DepartureTime,
                BasePrice = flight.BasePrice,
                AvailableSeats = flight.AvailableSeats
            };

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };

            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(dto),
                cacheOptions,
                cancellationToken);

            return dto;
        }
    }
}
