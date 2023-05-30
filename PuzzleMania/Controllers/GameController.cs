using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using PuzzleMania.ViewModels;

namespace PuzzleMania.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GameController(IGameRepository gameRepository, ITeamRepository teamRepository, IHttpContextAccessor httpContextAccessor)
        {
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StartGame()
        {
            //check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"You must be logged in to access this page.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            //retriving current user
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            //check if user has a team
            if (!await _teamRepository.CheckIfUserHasTeam(currentUserId))
            {
                TempData["Message"] = "You have not joined a team yet.";
                return RedirectToAction("JoinTeam", "Team");
            }

            bool isTeamIncomplete =  _gameRepository.IsTeamIncomplete(currentUserId);

            //check if current team is incomplete
            if (isTeamIncomplete)
            {
                TempData["Message"] = "Your team is incomplete, wait for users to join";
                return RedirectToAction("TeamStats", "Team");
            }

            //retriving the team of current user
            var team = await _teamRepository.GetTeamByUserId(currentUserId);


            var startGameViewModel = new StartGameViewModel
            {
                TeamName = team.TeamName
            };

            return View(startGameViewModel);
        }




    }
}
