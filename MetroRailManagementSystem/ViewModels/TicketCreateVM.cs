using System.ComponentModel.DataAnnotations;

namespace MetroRailManagementSystem.ViewModels
{
    public class TicketCreateVM
    {
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

        public string PaymentMethod { get; set; } = "bKash";
        public string? PaymentNumber { get; set; }   // bkash/rocket/nagad/bank account


    }
}
