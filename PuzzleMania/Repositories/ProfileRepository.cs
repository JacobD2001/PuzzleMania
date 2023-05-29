using Microsoft.AspNetCore.Identity;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Data;
using PuzzleMania.Helpers;
using PuzzleMania.Models;
using PuzzleMania.Areas.Identity.Data;
using System.Security.Claims;
using PuzzleMania.Interfaces;
//Currently not in use unnecessary code TOREMOVE
namespace PuzzleMania.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly PuzzleManiaContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<PuzzleManiaUser> _userManager;
        public ProfileRepository(PuzzleManiaContext context, IHttpContextAccessor httpContextAccessor, UserManager<PuzzleManiaUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        //method to get the user's profile
        public async Task<string> GetEmailOfCurrentUser(ClaimsPrincipal user)
        {
            var currentUser = await _userManager.GetUserAsync(user);
            return currentUser?.Email;
        }

    }
}
