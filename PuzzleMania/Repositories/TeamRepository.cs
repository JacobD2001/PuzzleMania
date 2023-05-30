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

        /*    public async Task<IEnumerable<string>> GetTeamMembers(int teamId)
            {
                var teamMembers = await _context.Teams
                    .Where(t => t.TeamId == teamId)
                    .SelectMany(t => t.UserId)
                    .Select(u => u .Split('@')[0]) // Extract the email without the "@" part
                    .ToListAsync();

                return teamMembers;
            }*/


        public async Task<IEnumerable<PuzzleManiaUser>> GetTeamMembers(string userId)
        {
            return await _context.Users
                .Where(u => u.Id == userId).ToListAsync();
        }



        // Find the user based on the current logged in user
        /*   public async Task<User> GetUserById(string currentUserId)
           {
               return await _context.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);
           }
   */


        public async Task<Team> GetByIdAsync(int teamId)
        {
            return await _context.Teams
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TeamId == teamId);
        }





    }
}
