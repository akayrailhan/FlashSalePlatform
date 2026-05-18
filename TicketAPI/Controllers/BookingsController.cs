using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
        {
            if (request.FlightId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Gecersiz ucus bilgisi." });
            }

            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(ClaimTypes.Name) ??
                User.Identity?.Name;

            // we get userId from the token claims, if it's not present we return unauthorized
            //safeguard to ensure that we have a userId to associate with the booking
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
