using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Commands;
using TicketAPI.DTOs;
using TicketAPI.Queries;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            if (request.FlightId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Gecersiz ucus bilgisi." });
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { success = false, message = "Kullanici bilgisi bulunamadi." });
            }

            var pnrCode = await _mediator.Send(
                new CreateBookingCommand(request.FlightId, userId));

            return Ok(new { success = true, pnrCode });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<BookingDto>>> GetBookings()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { success = false, message = "Kullanici bilgisi bulunamadi." });
            }

            var bookings = await _mediator.Send(new GetBookingsQuery(userId));
            return Ok(bookings);
        }

        private string? GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return null;
            }

            return User.FindFirstValue("sub")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("user_id")
                ?? User.Identity?.Name;
        }
    }
}
