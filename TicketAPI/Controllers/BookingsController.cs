using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Commands;
using TicketAPI.DTOs;

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

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            var userId =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            // Token'dan kullanici kimligini cekiyoruz.
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { success = false, message = "Kullanici bilgisi bulunamadi." });
            }

            var pnrCode = await _mediator.Send(
                new CreateBookingCommand(request.FlightId, userId));

            return Ok(new { success = true, pnrCode });
        }
    }
}
