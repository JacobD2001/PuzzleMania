using Microsoft.AspNetCore.Mvc;
using PuzzleMania.Helpers;
using PuzzleMania.Interfaces;
using PuzzleMania.Models;
using PuzzleMania.Repositories;
using PuzzleMania.ViewModels;

namespace PuzzleMania.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRiddleRepository _riddleRepository;

        public GameController(IGameRepository gameRepository, ITeamRepository teamRepository, IHttpContextAccessor httpContextAccessor, IRiddleRepository riddleRepository)
        {
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _httpContextAccessor = httpContextAccessor;
            _riddleRepository = riddleRepository;
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

        //TEST VVV TODO - Tu skończyłem
        [HttpPost]
        public async Task<IActionResult> StartGamePost()
        {
            // Get the current team ID
            //var curTeamId = await _teamRepository.GetByIdAsync(teamId);

            //1.Retrive current user
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();
            //2.Retrive the teamId of current user
            var curTeamId = await _teamRepository.GetTeamByUserId(currentUserId);
            //3. Assaign teamid to variable
            int teamId = curTeamId.TeamId;
            // Add a new game and get the generated gameId
            int gameId = _gameRepository.AddGame(teamId).GameId;

            List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds();

            if (availableRiddleIds.Count == 0)
            {
                // Handle the case when no riddles are available
                return NotFound();
            }

            // Select a random riddle ID
            Random random = new Random();
            int randomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];

            // Assign the random riddle ID to the current game
            await _riddleRepository.AssignRiddleId(gameId, randomRiddleId);
            // Redirect the user to the first riddle of the game
            return RedirectToAction("Riddle", new { gameId = gameId, riddleId = randomRiddleId });
        }

        [HttpGet("/game/{gameId}/riddle/{riddleId}")]
        public async Task<IActionResult> Riddle(int gameId, int riddleId)
        {
            // Check if it's a new game (no riddles assigned to the game yet)
            bool isNewGame = await _riddleRepository.IsNewGame(gameId);

            if (isNewGame)
            {
                // Assign the riddle ID to the current game
                await _riddleRepository.AssignRiddleId(gameId, riddleId);

                // Get the riddle information based on the gameId and riddleId
                var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

                // Handle the case when the riddle doesn't exist
                if (riddle == null)
                    return NotFound();

                // Redirect to the view
                return View(riddle);
             

            }
            return RedirectToAction("StartGame");
            

         
        }




        /*     [HttpGet("/game/{gameId}/riddle/{riddleId}")]
             public async Task<IActionResult> Riddle(int gameId, int riddleId)
             {
                 // Get the riddle information based on the gameId and riddleId
                 var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

                 if (riddle == null)           
                     return NotFound();


                 // Pass the riddle data to the view
                 return View(riddle);
             }*/










    }






}
