using System.Drawing.Drawing2D;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Models;

namespace PuzzleMania.Interfaces
{
    public interface ITeamRepository
    {
        Task<bool> CheckIfUserHasNullTeamId(string currentUserId);
        //nie widzi teamow bo nie ma ich w modelach tu a jest w bazie team model TODO
        Task<IEnumerable<Team>> GetAll();
        Task<IEnumerable<Team>> GetIncompleteTeams();
        bool Add(Team team);
        //Task<Team> GetByIdAsync(int id);
        Task<Team> GetTeamByName(string teamName);
        public bool Save();
        public bool Update(Team team);
        public void AddUserToTeam(string userId, int teamId);
        Task  IncreaseTeamSize(int teamId);
        Task<Team> GetTeamByUserId(string userId);
        Task<IEnumerable<PuzzleManiaUser>> GetTeamMembers(int teamId);




    }
}
