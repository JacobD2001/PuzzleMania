using System.Security.Claims;

namespace PuzzleMania.Interfaces
{
    public interface IProfileRepository
    {
        public Task<string> GetEmailOfCurrentUser(ClaimsPrincipal user);

    }
}
