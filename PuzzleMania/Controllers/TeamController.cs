using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuzzleMania.Data;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using PuzzleMania.Repositories;

namespace PuzzleMania.Controllers
{
    public class TeamController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITeamRepository _teamRepository;
        private readonly PuzzleManiaContext _context;

        public TeamController(IHttpContextAccessor httpContextAccessor, ITeamRepository teamRepository, PuzzleManiaContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _teamRepository = teamRepository;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> JoinTeam()
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            // Check if the user has already joined a team
            if (_teamRepository.CheckIfUserHasNullTeamId(currentUserId))// returns false meaning that curuser has joined a team
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

        //this method shows the team stats(names) and gets called when there are incomplete teams and user has to choose a team
        [HttpGet]
        public async Task<IActionResult> TeamStats()
        {
            var teamStats = await _teamRepository.GetAll();
            return View(teamStats);
        }


        [HttpPost]
        public async Task<IActionResult> JoinTeam(string teamName)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();


            // Logic to handle the form submission
            if (!string.IsNullOrEmpty(teamName))
            {
                // Create the new team with the provided name and assaigning user to a team
                var newTeam = new Team
                {
                    TeamName = teamName,
                    UserId = currentUserId
                };

                _teamRepository.Add(newTeam);
                TempData["Message"] = "Team created successfully!";
                return RedirectToAction("TeamStats");
            }
            else
            {
                // Logic to add the user to the selected team
                var user = _context.Users.SingleOrDefault(u => u.Id == currentUserId);

                if (user != null)
                {
                    var teamToJoin = await _teamRepository.GetTeamByName(teamName);  // Retrieve the team the user wants to join based on their selection

                    if (teamToJoin != null)
                    {
                        //Change this code to use the repository
                        user.TeamId = teamToJoin.TeamId; // Assign the user to the selected team
                         _teamRepository.Save();

                        TempData["Message"] = "Joined the team successfully!";
                        return RedirectToAction("TeamStats");

                    }
                    else
                    {
                        TempData["Message"] = "Selected team does not exist!";
                        return View();
                    }
                }

                return RedirectToAction("TeamStats");
            }


        }
    }
}
