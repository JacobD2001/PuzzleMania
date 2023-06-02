using System.ComponentModel;

namespace PuzzleMania.Models
{
    public class UserPoints
    {
        public int UserPointsId { get; set; }
        public string UserId { get; set; }
        //public User User { get; set; }

        [DefaultValue(10)]
        public int Points { get; set; }
    }
}
