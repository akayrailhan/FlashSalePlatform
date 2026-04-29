using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // Database connection
        public FlightsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] Flight newFlight)
        {
            _context.Flights.Add(newFlight);
            await _context.SaveChangesAsync();
            return Ok(newFlight);
        }

        // Flight information without sensitive data
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(Guid id)
        {
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
            var newOrder = new Order
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