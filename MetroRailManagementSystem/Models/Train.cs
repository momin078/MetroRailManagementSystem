using System.ComponentModel.DataAnnotations;

namespace MetroRailManagementSystem.Models
{
    public class Train
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Range(1, 5000)]
        public int TotalSeats { get; set; } = 200;

        [Range(0, 5000)]
        public int AvailableSeats { get; set; } = 200;

        public bool IsActive { get; set; } = true;
    }
}
