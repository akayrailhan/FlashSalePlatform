using System.ComponentModel.DataAnnotations;

namespace TicketAPI.Models
{
    public class Flight
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty; // Örn: TK1903
        public string Origin { get; set; } = string.Empty;       // Örn: IST
        public string Destination { get; set; } = string.Empty;  // Örn: AMS
        public DateTime DepartureTime { get; set; }
        public decimal BasePrice { get; set; }
        public int TotalCapacity { get; set; }
        public int AvailableSeats { get; set; }

        // PostgreSQL için Hayati Öneme Sahip Concurrency (Eşzamanlılık) Kontrolü
        [Timestamp]
        public uint Version { get; set; }
    }
}