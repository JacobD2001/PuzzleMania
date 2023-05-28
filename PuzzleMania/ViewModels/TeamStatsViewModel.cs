using PuzzleMania.Areas.Identity.Data;

namespace PuzzleMania.ViewModels
{
    public class TeamStatsViewModel
    {
        public string TeamName { get; set; }
        public int TeamSize { get; set; }
        public IEnumerable<PuzzleManiaUser> TeamMembers { get; set; }
        public int TotalPoints { get; set; }
    }
}
