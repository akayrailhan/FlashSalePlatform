using MediatR;
using Microsoft.AspNetCore.Mvc;
using TicketAPI.Commands;
using TicketAPI.DTOs;
using TicketAPI.Models;
using TicketAPI.Queries;

namespace TicketAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FlightsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight([FromBody] Flight newFlight)
        {
            var created = await _mediator.Send(new CreateFlightCommand(newFlight));
            return Ok(created);
        }

        // Flight information without sensitive data (Redis Cache Destekli)
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDto>> GetFlight(Guid id)
        {
            var dto = await _mediator.Send(new GetFlightQuery(id));
            if (dto == null) return NotFound("Uçuş bulunamadı.");

            return Ok(dto);
        }

        [HttpPost("{id}/buy")]
        public async Task<IActionResult> BuyTicket(Guid id, [FromBody] BuyTicketRequestDto request)
        {
            var result = await _mediator.Send(new BuyTicketCommand(id, request.UserEmail, request.SeatCount));
            return StatusCode(result.StatusCode, new { result.Message, result.OrderId });
        }
    }
}