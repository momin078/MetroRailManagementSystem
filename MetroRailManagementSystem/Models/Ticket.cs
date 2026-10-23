using System;
using System.ComponentModel.DataAnnotations;

namespace MetroRailManagementSystem.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string PassengerName { get; set; } = string.Empty;

        [Required]
        public int FromStationId { get; set; }

        [Required]
        public int ToStationId { get; set; }

        [Required]
        public int TrainId { get; set; }

        [Range(1, 20)]
        public int Quantity { get; set; } = 1;

        [Range(0, 100000)]
        public decimal TotalFare { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }

        public bool IsCancelled { get; set; } = false;
        public DateTime? CancelledAt { get; set; }
        public string? CancelledByUserId { get; set; }

        public string PaymentMethod { get; set; } = "bKash";
        public string PaymentStatus { get; set; } = "Paid"; // simple demo
        public int DistanceKm { get; set; } // store calculated km for history
        public decimal FarePerKm { get; set; } = 5m;
        public string? PaymentNumber { get; set; }




    }
}
