using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Models
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty; //  TK1903
        public string Origin { get; set; } = string.Empty;       //  IST
        public string Destination { get; set; } = string.Empty;  //  AMS
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalCapacity { get; set; }
        public int AvailableSeats { get; set; }

        // Concurrency control for PostgreSQL
        [Timestamp]
        [ConcurrencyCheck]
        public uint Version { get; set; }
    }
}