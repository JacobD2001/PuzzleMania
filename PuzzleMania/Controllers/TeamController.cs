using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Helpers;
using PuzzleMania.Repositories;

namespace PuzzleMania.Controllers
{
    public class TeamController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TeamRepository _teamRepository;

        public TeamController(IHttpContextAccessor httpContextAccessor, TeamRepository teamRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _teamRepository = teamRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Join()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Check if the user has already joined a team
            if (!_teamRepository.CheckIfUserHasNullTeamId(currentUserId))// returns false meaning that curuser has joined a team
            {
                TempData["Message"] = "You have already joined a team.";
                return RedirectToAction("TeamStats");
            }

            // Retrieve the list of incomplete teams and than pass it to the view
            var incompleteTeams = await _teamRepository.GetIncompleteTeams();

            if (incompleteTeams.Count() > 0) //checks if there are any incomplete teams, if there are incomplete teams than user has to choose a team
            {
                return View("ChooseTeam", incompleteTeams);
            }

            // No incomplete teams & user has no team, show the form to create a new team
            return View("CreateTeam");
        }

        public async Task<IActionResult> TeamStats()
        {
            var teamStats = await _teamRepository.GetAll();
            return View(teamStats);
        }


    }
}
