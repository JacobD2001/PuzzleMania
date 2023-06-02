using Microsoft.EntityFrameworkCore;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Data;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using System.Drawing.Drawing2D;
using System.Linq;

namespace PuzzleMania.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly PuzzleManiaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeamRepository(PuzzleManiaContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //returns true if user has already joined a team
        public async Task<bool> CheckIfUserHasTeam(string currentUserId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(t => t.UserId == currentUserId);
            return team != null;
        }

        public async Task<IEnumerable<Team>> GetAll()
        {
            return await _context.Teams.ToListAsync();
        }



        public async Task<IEnumerable<Team>> GetIncompleteTeams()
        {
            return await _context.Teams
                .Where(t => t.TeamSize < 2)
                .ToListAsync();
        }


        public bool Add(Team team)
        {
            _context.Teams.Add(team);
            return _context.SaveChanges() > 0; // at least one change needs to be made to return true
        }

        // Method to retrieve the team the user wants to join based on their selection

        public async Task<Team> GetTeamByName(string teamName)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.TeamName == teamName);
        }

        public bool Save()
        {
            return (_context.SaveChanges() > 0);
        }

        public bool Update(Team team)
        {
            _context.Update(team);
            return Save();
        }

        public async Task<bool> AddUserToTeam(string currentUserId, int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                team.UserId = currentUserId;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task IncreaseTeamSize(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                team.TeamSize += 1;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Team> GetTeamByUserId(string userId)
        {
            return await _context.Teams
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<IEnumerable<PuzzleManiaUser>> GetTeamMembers(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId).ToListAsync();
        }

        public async Task<Team> GetByIdAsync(int teamId)
        {
            return await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TeamId == teamId);
        }

        public async Task<int> GetTotalPointsForTeamAsync(string userId)
        {
            var teamPoints = await _context.Teams
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            return teamPoints.TotalPoints;
        }

        //method to save the total points for the team
        public async Task SaveTotalPointsForTeamAsync(string userId, int totalPoints)
        {
            var team = await _context.Teams
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            team.TotalPoints = totalPoints;
            await _context.SaveChangesAsync();
        }

        //method that will return the teamid for the user
        public async Task<int> GetTeamIdForUserAsync(string userId)
        {
            var team = await _context.Teams
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            return team.TeamId;
        }



    }
}
