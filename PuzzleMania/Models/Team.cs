using System.ComponentModel.DataAnnotations;

namespace PuzzleMania.Models
{
    public class Team
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        [Range(0, 2, ErrorMessage = "Team size must be 0, 1, or 2.")]
        public int TeamSize { get; set; }
        public string UserId { get; set;}
        public int TotalPoints { get; set; }


    }
}
