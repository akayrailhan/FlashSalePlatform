using Microsoft.AspNetCore.Mvc;
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

        // Veritabanı bağlantımızı (Supabase) buraya enjekte ediyoruz
        public FlightsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. UÇ NOKTA: Sadece senin kampanya oluşturmak için kullanacağın "Gizli" metod
        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] Flight newFlight)
        {
            _context.Flights.Add(newFlight);
            await _context.SaveChangesAsync();
            return Ok(newFlight);
        }

        // 2. UÇ NOKTA: React'in uçuş bilgisini ve "Kalan Koltuk" sayısını çekeceği metod
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(Guid id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null) return NotFound("Uçuş bulunamadı.");

            // Veritabanı modelini DTO'ya dönüştür (Gizli bilgileri sakla)
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

            return Ok(dto);
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyTicket(Guid id, [FromBody] BuyTicketRequestDto request)
        {
            // PROJENİN EN CAN ALICI NOKTASI BURASI!
            // Çifte satışı (Overselling) engelleyen Optimistic Concurrency kodlarını bir sonraki adımda buraya yazacağız.
            return Ok("Satın alma altyapısı hazır, mantık buraya eklenecek.");
        }
    }
}