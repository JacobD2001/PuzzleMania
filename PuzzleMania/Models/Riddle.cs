using System.ComponentModel.DataAnnotations.Schema;

namespace PuzzleMania.Models
{
    public class Riddle
    {
        public int RiddleId { get; set; } //PK

        public int GameId { get; set; } //FK REF games.GameId
        public Game Game { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
