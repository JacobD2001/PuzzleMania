using PuzzleMania.Models;

namespace PuzzleMania.Interfaces
{
    public interface IUserPointsRepository
    {
        Task<int> GetTotalPointsForUserAsync(string userId);
        bool Add(UserPoints userPoints);

    }
}
