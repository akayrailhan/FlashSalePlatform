using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TicketAPI.Data;
using TicketAPI.DTOs;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache; // Redis motorumuz eklendi

        // Database ve Cache bağlantısı (Dependency Injection)
        public FlightsController(AppDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] Flight newFlight)
        {
            _context.Flights.Add(newFlight);
            await _context.SaveChangesAsync();
            return Ok(newFlight);
        }

        // Flight information without sensitive data (Redis Cache Destekli)
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(Guid id)
        {
            string cacheKey = $"flight_{id}";

            // 1. Önce Redis'e sor (Milisaniyelik hız)
            var cachedFlight = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedFlight))
            {
                // Önbellekte varsa JSON'dan DTO'ya çevir ve veritabanına hiç gitmeden dön
                return Ok(JsonSerializer.Deserialize<FlightDto>(cachedFlight));
            }

            // 2. Önbellekte yoksa (Cache Miss) veritabanına git
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null) return NotFound("Uçuş bulunamadı.");

            // Entity-to-DTO mapping
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

            // 3. Veritabanından geleni 2 dakikalığına Redis'e kaydet
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), cacheOptions);

            return Ok(dto);
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyTicket(Guid id, [FromBody] BuyTicketRequestDto request)
        {
            // 1. Load the flight from the database
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return NotFound("Uçuş bulunamadı.");

            // 2. Logical check (enough seats?)
            if (flight.AvailableSeats < request.SeatCount)
            {
                return BadRequest(new { Message = $"Yetersiz koltuk. Sadece {flight.AvailableSeats} koltuk kaldı." });
            }

            // 3. Update in memory (decrease seats, bump version)
            flight.AvailableSeats -= request.SeatCount;
            flight.Version += 1; // Bump version on each purchase

            // 4. Create the order record
            var newOrder = new TicketAPI.Models.Order
            {
                FlightId = flight.Id,
                UserEmail = request.UserEmail,
                SeatCount = request.SeatCount,
                CreatedAt = DateTime.UtcNow
            };
            _context.Orders.Add(newOrder);

            // 5. Try to persist to the database
            try
            {
                // If someone bought seats before we save (version changed),
                // EF throws DbUpdateConcurrencyException.
                await _context.SaveChangesAsync();

                // KRİTİK DOKUNUŞ: Bilet satıldığı ve koltuk azaldığı için Redis'teki eski veriyi çöpe atıyoruz.
                // Bir sonraki "GetFlight" isteği mecburen veritabanına gidip güncel koltuk sayısını alacak.
                await _cache.RemoveAsync($"flight_{id}");

                return Ok(new { Message = "Bilet başarıyla satın alındı!", OrderId = newOrder.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                // Race condition caught. Overselling prevented.
                return StatusCode(409, new { Message = "Siz işlemi tamamlarken biletler tükendi veya başkası tarafından alındı. Lütfen tekrar deneyin." });
            }
        }
    }
}