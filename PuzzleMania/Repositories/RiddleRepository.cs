using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;

namespace PuzzleMania.Repositories
{
    public class RiddleRepository : IRiddleRepository
    {
        private readonly PuzzleManiaContext _context;

        public RiddleRepository(PuzzleManiaContext context)
        {
            _context = context;
        }

        public bool Add(Riddle riddle)
        {
            _context.Add(riddle);
            return Save();
        }

        public bool Delete(Riddle riddle)
        {
            _context.Remove(riddle);
            return Save();
        }

        public bool Save()
        {
            return (_context.SaveChanges() > 0);
        }

        public bool Update(Riddle riddle)
        {
            _context.Update(riddle);
            return Save();
        }

        public async Task<IEnumerable<Riddle>> GetAll()
        {
            return await _context.Riddles.ToListAsync();
        }

        public async Task<Riddle> GetByIdAsync(int riddleId)
        {
            return await _context.Riddles
                .FirstOrDefaultAsync(r => r.RiddleId == riddleId);
        }

        public async Task<int?> GetRiddleIdByGameId(int gameId)
        {
            var riddle = await _context.Riddles.FirstOrDefaultAsync(r => r.GameId == gameId);
            return riddle?.RiddleId;
        }

        public async Task<List<int>> GetAvailableRiddleIds()
        {
            return await _context.Riddles.Select(r => r.RiddleId).ToListAsync();
        }
        public async Task<List<int>> GetAvailableRiddleIds(int gameId)
        {
            // Retrieve the list of riddle IDs that are not assigned to any game or assigned to the specified game
            var availableRiddleIds = await _context.Riddles
                .Where(r => r.GameId == gameId)
                .Select(r => r.RiddleId)
                .ToListAsync();

            return availableRiddleIds;
        }



        public async Task AssignRiddleId(int gameId, int riddleId)
        {
            var riddle = await _context.Riddles.FindAsync(riddleId);

            if (riddle != null)
            {
                riddle.GameId = gameId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsNewGame(int gameId)
        {
            // Check if any riddle is associated with the given gameId
            return await _context.Riddles.AnyAsync(r => r.GameId == gameId);
        }

       


    }
}
