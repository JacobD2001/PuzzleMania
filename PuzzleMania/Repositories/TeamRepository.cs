using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using System.Drawing.Drawing2D;
using System.Linq;

namespace PuzzleMania.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly PuzzleManiaContext _context;

        public TeamRepository(PuzzleManiaContext context)
        {
            _context = context;
        }

        public bool CheckIfUserHasNullTeamId(string currentUserId)
        {
            var user = _context.Users.SingleOrDefaultAsync(u => u.Id == currentUserId);

            // If User exists
            if (user != null)
            {
                var teamId = _context.Users
                .Where(u => u.Id == currentUserId)
                .Select(u => u.TeamId)
                .FirstOrDefaultAsync();

                if (teamId == null)
                {
                    // User has a NULL value in the TeamId column
                    return true;
                }
            }

            // User does not have a NULL value in the TeamId column or user not found = User has already joined a team
            return false;
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Select(t => t.TeamName);
        }


        public async Task<IEnumerable<Team>> GetIncompleteTeams()
        {
            var incompleteTeams = await _context.Users
                .GroupBy(u => u.TeamId) // Group users by TeamId
                .Where(g => g.Count() <= 1) // Filter groups where the count is less than or equal to 1
                .Select(g => g.Key) // Select the TeamId of incomplete teams
                .ToListAsync();

            var teams = await _context.Teams
                .Where(t => incompleteTeams.Contains(t.TeamId)) // Retrieve the incomplete teams
                .ToListAsync();

            return teams;
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



        /*   public async Task<Team> GetByIdAsync(int id)
           {
               return await _context.Teams
                   .FirstOrDefaultAsync(t => t.TeamId == id);
           }*/


    }
}
