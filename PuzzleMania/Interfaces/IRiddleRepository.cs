using PuzzleMania.Models;
using System.Drawing.Drawing2D;

namespace PuzzleMania.Interfaces
{
    public interface IRiddleRepository
    {
        //You must implement a REST API that allows the user to create a riddle, list all riddles, update a riddle and delete a riddle.\

        bool Add(Riddle riddle);
        bool Update(Riddle riddle);
        bool Delete(Riddle riddle);
        bool Save();
        Task<IEnumerable<Riddle>> GetAll();
        Task<Riddle> GetByIdAsync(int riddleId);
        Task<int?> GetRiddleIdByGameId(int gameId);
        Task<List<int>> GetAvailableRiddleIds();
        Task AssignRiddleId(int gameId, int riddleId);
        Task<bool> IsNewGame(int gameId);
        Task<List<int>> GetAvailableRiddleIds(int gameId);

    }
}
