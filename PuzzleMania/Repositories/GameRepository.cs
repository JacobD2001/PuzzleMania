using PuzzleMania.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;

namespace PuzzleMania.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly PuzzleManiaContext _context;

        public GameRepository(PuzzleManiaContext context)
        {
            _context = context;
        }

        //returns true if team is incomplete otherwise false
        public bool IsTeamIncomplete(string userId)
        {
            var team = _context.Teams.FirstOrDefault(t => t.UserId == userId);
            return team.TeamSize < 2;
        }

        public Game AddGame(int teamId)
        {
            Game game = new Game { TeamId = teamId };
            _context.Games.Add(game);
            _context.SaveChanges();
            return game;
        }

    }
}
