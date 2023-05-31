using PuzzleMania.Models;

namespace PuzzleMania.Interfaces
{
    public interface IGameRepository
    {
        bool IsTeamIncomplete(string userId);
        Game AddGame(int teamId);
        Task<Game> GetGameByIdAsync(int gameId);

    }
}
