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

        public async Task<Riddle> GetByIdAsync(int gameId, int id)
        {
            return await _context.Riddles
                .FirstOrDefaultAsync(r => r.RiddleId == id);
        }
    }
}
