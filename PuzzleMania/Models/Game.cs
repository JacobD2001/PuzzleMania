using System.ComponentModel.DataAnnotations.Schema;

namespace PuzzleMania.Models
{
    public class Game
    {
        public int GameId { get; set; } //PK
        public int TeamId { get; set; } //FK REF teams.TeamId
        public Team Team { get; set; } //nav prop for creating FK ref
       
    }
}
