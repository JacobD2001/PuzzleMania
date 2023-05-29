using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.ViewModels;

namespace PuzzleMania.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly UserManager<PuzzleManiaUser> _userManager;

        public ProfileController(IProfileRepository profileRepository, UserManager<PuzzleManiaUser> userManager)
        {
            _profileRepository = profileRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Redirect to the login page if the user is not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"You must be logged in to access this page.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            //var email = await _profileRepository.GetEmailOfCurrentUser(User);

            var profileViewModel = new ProfileViewModel
            {
                Email = currentUser.Email
            };

            return View(profileViewModel);
        }

    }
}
