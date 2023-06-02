using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;

namespace PuzzleMania.Repositories
{
    public class UserPointsRepository : IUserPointsRepository
    {

        private readonly PuzzleManiaContext _context;
        public UserPointsRepository(PuzzleManiaContext context)
        {
            _context = context;
        }
        public async Task<int> GetTotalPointsForUserAsync(string userId)
        {
            var userPoints = await _context.UserPoints
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            return userPoints.Points;
        }

        public bool Add(UserPoints userPoints)
        {
            _context.UserPoints.Add(userPoints);
            return _context.SaveChanges() > 0; // at least one change needs to be made to return true
        }


        /*      public async Task AssignPointsToUserAsync(string userId, int points)
              {
                  var userPoints = await _context.UserPoints
                      .FirstOrDefaultAsync(x => x.UserId == userId);

                  if (userPoints != null)
                  {
                      userPoints.Points += points;
                  }
                  else
                  {
                      userPoints = new UserPoints
                      {
                          UserId = userId,
                          Points = points
                      };
                      _context.UserPoints.Add(userPoints);
                  }

                  await _context.SaveChangesAsync();
              }*/




    }
}
