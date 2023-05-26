using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using System.Drawing.Drawing2D;

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


    }
}
