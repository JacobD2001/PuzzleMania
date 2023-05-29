using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Areas.Identity.Data;
using PuzzleMania.Interfaces;
using PuzzleMania.ViewModels;
using System;

namespace PuzzleMania.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileRepository _profileRepository;
        private readonly UserManager<PuzzleManiaUser> _userManager;
        private readonly IPhotoService _photoService;



        public ProfileController(IProfileRepository profileRepository, UserManager<PuzzleManiaUser> userManager, IPhotoService photoService)
        {
            _profileRepository = profileRepository;
            _userManager = userManager;
            _photoService = photoService;
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
                Email = currentUser.Email,
                URL = currentUser.Image             
            };

            return View(profileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel profileViewModel)
        {
            // Redirect to the login page if the user is not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"You must be logged in to access this page.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var currentUser = await _userManager.GetUserAsync(User);

            if (!string.IsNullOrEmpty(currentUser.Image))
            {
                _ = _photoService.DeletePhotoAsync(currentUser.Image);
            }

            var result = await _photoService.AddPhotoAsync(profileViewModel.ProfilePicture);


            if (ModelState.IsValid)
            {
                currentUser.Image = result.Url.ToString();
                currentUser.Email = profileViewModel.Email;
                await _userManager.UpdateAsync(currentUser);
                TempData["Message"] = "Profile picture updated successfully.";
                return View("Profile");
            }
            else if (!ModelState.IsValid)
            {
                return View(profileViewModel);
            }
            return View(profileViewModel);
        }

    }
}
