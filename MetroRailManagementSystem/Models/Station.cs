using System.ComponentModel.DataAnnotations;

namespace MetroRailManagementSystem.Models
{
    public class Station
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string Code { get; set; } = string.Empty; // e.g., DHA, CTG

        public bool IsActive { get; set; } = true;

        public int DistanceKm { get; set; } // distance from start point (e.g., Uttara = 0)


    }
}
