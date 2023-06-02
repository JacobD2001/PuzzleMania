using System.Drawing.Drawing2D;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Models;

namespace PuzzleMania.Interfaces
{
    public interface ITeamRepository
    {
        Task<bool> CheckIfUserHasTeam(string currentUserId);
        Task<IEnumerable<Team>> GetAll();
        Task<IEnumerable<Team>> GetIncompleteTeams();
        bool Add(Team team);
        Task<Team> GetTeamByName(string teamName);
        public bool Save();
        public bool Update(Team team);
        Task<bool> AddUserToTeam(string currentUserId, int teamId);
        Task  IncreaseTeamSize(int teamId);
        Task<Team> GetTeamByUserId(string userId);
        Task<IEnumerable<PuzzleManiaUser>> GetTeamMembers(string userId);
        Task<Team> GetByIdAsync(int teamId);
        Task<int> GetTotalPointsForTeamAsync(string userId);
        Task SaveTotalPointsForTeamAsync(string userId, int totalPoints);
        Task<int> GetTeamIdForUserAsync(string userId);





    }
}
