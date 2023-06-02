using Microsoft.AspNetCore.Http;
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
        private readonly IUserPointsRepository _totalPointsRepository;

        public GameController(IGameRepository gameRepository, ITeamRepository teamRepository, IHttpContextAccessor httpContextAccessor, IRiddleRepository riddleRepository, IUserPointsRepository totalPointsRepository)
        {
            _gameRepository = gameRepository;
            _teamRepository = teamRepository;
            _httpContextAccessor = httpContextAccessor;
            _riddleRepository = riddleRepository;
            _totalPointsRepository = totalPointsRepository;
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

            /*List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds();

            if (availableRiddleIds.Count == 0)
                // Handle the case when no riddles are available
                return NotFound();*/


            // Select a random riddle ID
            // Random random = new Random();
            //int randomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];
            int randomRiddleId = await GetNextRandomRiddleId();

            // Assign the random riddle ID to the current game
            await _riddleRepository.AssignRiddleId(gameId, randomRiddleId);
            // Redirect the user to the first riddle of the game
            return RedirectToAction("Riddle", new { gameId = gameId, riddleId = randomRiddleId });
        }

        [HttpGet("/game/{gameId}/riddle/{riddleId}")]
        public async Task<IActionResult> Riddle(int gameId, int riddleId, string? answer = null)
        {
            //TEST
            // Check if the method was accessed from SubmitAnswer
           // bool accessedFromSubmitAnswer = TempData.ContainsKey("AccessedFromSubmitAnswer") && (bool)TempData["AccessedFromSubmitAnswer"];

            // Clear the TempData value
            TempData.Remove("AccessedFromSubmitAnswer");


            // Get the riddle information based on the gameId and riddleId
            var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

            // Handle the case when the riddle doesn't exist
            if (riddle == null)
                return NotFound();

        /*    if(accessedFromSubmitAnswer)
            {
                // Set a ViewBag property to indicate it was accessed from SubmitAnswer
                ViewBag.AccessedFromSubmitAnswer = true;

                return RedirectToAction("Riddle", new { gameId = gameId, riddleId = riddleId });
            }*/

         // Pass the riddle data to the view
            return View("Riddle", riddle);
        }

        [HttpPost("/game/{gameId}/riddle/{riddleId}")]
        public async Task<IActionResult> SubmitAnswer(int gameId, int riddleId, string answer)
        {
            // Get the riddle information based on the gameId and riddleId
            var riddle = await _riddleRepository.GetByIdAsync(gameId, riddleId);

            // Handle the case when the riddle doesn't exist
            if (riddle == null)
                return NotFound();

            //Get the current user Id
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            //Get the value of userPoints from currentstate
            int? userPoints = _httpContextAccessor.HttpContext.Session.GetInt32("UserPoints");

            bool isFirstAccessed = IsFirstTimeAccessed();

            // Check if userPoints is null (first iteration of SubmitAnswer) //TODO - logic here
            if (isFirstAccessed)
            {
                // Retrieve the initial user points value from the database
                userPoints = await _totalPointsRepository.GetTotalPointsForUserAsync(curUserId);

                // Store the initial user points in session state
                _httpContextAccessor.HttpContext.Session.SetInt32("UserPoints", userPoints.Value);

                // Remove the flag from session state for each new game session
                //_httpContextAccessor.HttpContext.Session.Remove("FirstTimeAccess");
            }
            else
            {
                // Retrieve the user points from session state
                userPoints = _httpContextAccessor.HttpContext.Session.GetInt32("UserPoints");
            }

            // Check if the submitted answer is correct
            bool isCorrect = CheckAnswer(answer, riddle.Answer);

            //Get new random riddle
            int nextRandomRiddleId = await GetNextRandomRiddleId(); 

            // Retrieve the completedRiddlesCount from session state
            int? completedRiddlesCount = _httpContextAccessor.HttpContext.Session.GetInt32("CompletedRiddlesCount") ?? 0;

            // Increment the count of completed riddles
            completedRiddlesCount++;

            // Store the current completedRiddlesCount in session state
            _httpContextAccessor.HttpContext.Session.SetInt32("CompletedRiddlesCount", completedRiddlesCount.Value);


            // Retrieve the team points from the database
            int? teamPoints = await _teamRepository.GetTotalPointsForTeamAsync(curUserId); //tutaj bierze teampointsy gdzie do teamu nie jest jeszcze nic przypisanego daltego teampoints = 0

            //conversion int? to int
            int pointsToSave = teamPoints ?? 0;

            if (isCorrect)
            {
                // Display a success message
                ViewBag.Message = "Correct answer!";


                //increment points by 10
                userPoints += 10;

                // Store the changed user points in session state
                _httpContextAccessor.HttpContext.Session.SetInt32("UserPoints", userPoints.Value);

                // Check if all riddles have been completed
                if (completedRiddlesCount >= 3) 
                {
                    completedRiddlesCount = 0;
                    // Store the current completedRiddlesCount in session state
                    _httpContextAccessor.HttpContext.Session.SetInt32("CompletedRiddlesCount", completedRiddlesCount.Value);
                    teamPoints = teamPoints + userPoints;
                    await _teamRepository.SaveTotalPointsForTeamAsync(curUserId, pointsToSave);

                    userPoints = 0;
                    // Redirect to the finish game view
                    return RedirectToAction("FinishGame");
                }

                // Set the ViewBag properties
                ViewBag.ShowNextButton = true;
                ViewBag.NextRiddleId = nextRandomRiddleId;


                //assaigning next riddle to a game
                await _riddleRepository.AssignRiddleId(gameId, nextRandomRiddleId);

                return await Riddle(gameId, nextRandomRiddleId);
            }
            else
            {
                // Display the correct answer
                ViewBag.Message = "Incorrect answer. The correct answer is: " + riddle.Answer;

                // Set the ViewBag properties
                ViewBag.ShowNextButton = true;
                ViewBag.NextRiddleId = nextRandomRiddleId;

                userPoints -= 10;

                // Store the current user points in session state
                _httpContextAccessor.HttpContext.Session.SetInt32("UserPoints", userPoints.Value);

                if (completedRiddlesCount >= 3 || userPoints == 0) 
                {
                    completedRiddlesCount = 0;
                    // Store the current completedRiddlesCount in session state
                    _httpContextAccessor.HttpContext.Session.SetInt32("CompletedRiddlesCount", completedRiddlesCount.Value);
                    teamPoints = teamPoints + userPoints;
                    await _teamRepository.SaveTotalPointsForTeamAsync(curUserId, pointsToSave);
                    userPoints = 0;
                    // Redirect to the finish game action
                    return RedirectToAction("FinishGame");
                }
            }


            await _riddleRepository.AssignRiddleId(gameId, nextRandomRiddleId);
            return await Riddle(gameId, nextRandomRiddleId);
        }

        private bool IsFirstTimeAccessed()
        {
            bool isFirstTime = false;

            // Check if the flag exists in session state
            if (!_httpContextAccessor.HttpContext.Session.TryGetValue("FirstTimeAccess", out byte[] flag))
            {
                // Flag doesn't exist, it's the first time accessing
                isFirstTime = true;

                // Set the flag in session state
                _httpContextAccessor.HttpContext.Session.Set("FirstTimeAccess", new byte[1]);
            }



            return isFirstTime;
        }





        [HttpGet]
        public async Task<IActionResult> FinishGame()
        {
            var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();


            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = $"You must be logged in to access this page.";
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var team = await _teamRepository.GetTeamByUserId(curUserId);
            return View(team);
        }

        //this method check if the answer is correct
        private bool CheckAnswer(string submittedAnswer, string correctAnswer)
        {
            bool isCorrect = string.Equals(submittedAnswer, correctAnswer, StringComparison.OrdinalIgnoreCase);
            return isCorrect;

        }

        //this method selects the next random riddle
        private async Task<int> GetNextRandomRiddleId() 
        {
            List<int> availableRiddleIds = await _riddleRepository.GetAvailableRiddleIds();

            if (availableRiddleIds.Count == 0)
                throw new Exception("No riddles are available.");


            // Select a random riddle ID
            Random random = new Random();
            int randomRiddleId = availableRiddleIds[random.Next(availableRiddleIds.Count)];

            // Assign the random riddle ID to the current game

            return randomRiddleId;
        }


    }






}
